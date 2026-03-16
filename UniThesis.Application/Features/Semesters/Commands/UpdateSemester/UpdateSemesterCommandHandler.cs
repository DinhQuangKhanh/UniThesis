using UniThesis.Application.Common.Abstractions;
using MediatR;
using UniThesis.Domain.Aggregates.SemesterAggregate;
using UniThesis.Domain.Common.Exceptions;
using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Application.Features.Semesters.Commands.UpdateSemester;

/// <summary>
/// Handles the UpdateSemesterCommand by validating and updating an existing semester.
/// The domain's EnsureUpcoming guard rejects updates for Ongoing/Ended semesters.
/// </summary>
public class UpdateSemesterCommandHandler : ICommandHandler<UpdateSemesterCommand>
{
    private readonly ISemesterRepository _semesterRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSemesterCommandHandler(
        ISemesterRepository semesterRepository,
        IUnitOfWork unitOfWork)
    {
        _semesterRepository = semesterRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateSemesterCommand request, CancellationToken cancellationToken)
    {
        var semester = await _semesterRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Semester), request.Id);

        // Domain guards will throw if semester is not Upcoming
        semester.UpdateDetails(request.Name, request.Description);
        semester.UpdateDates(request.StartDate, request.EndDate);

        // Update phase dates if provided
        if (request.Phases is { Count: > 0 })
        {
            foreach (var phase in request.Phases)
            {
                semester.UpdatePhaseDates(phase.Id, phase.StartDate, phase.EndDate);
            }
        }

        _semesterRepository.Update(semester);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
