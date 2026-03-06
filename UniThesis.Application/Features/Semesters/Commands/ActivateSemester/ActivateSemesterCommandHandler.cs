using MediatR;
using UniThesis.Application.Common.Abstractions;
using UniThesis.Domain.Aggregates.SemesterAggregate;
using UniThesis.Domain.Common.Exceptions;
using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Application.Features.Semesters.Commands.ActivateSemester;

/// <summary>
/// Handles the ActivateSemesterCommand.
/// Loads the semester and transitions it from Upcoming to Active.
/// </summary>
public class ActivateSemesterCommandHandler : ICommandHandler<ActivateSemesterCommand>
{
    private readonly ISemesterRepository _semesterRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateSemesterCommandHandler(
        ISemesterRepository semesterRepository,
        IUnitOfWork unitOfWork)
    {
        _semesterRepository = semesterRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ActivateSemesterCommand request, CancellationToken cancellationToken)
    {
        var semester = await _semesterRepository.GetByIdAsync(request.SemesterId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Semester), request.SemesterId);

        semester.Activate();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
