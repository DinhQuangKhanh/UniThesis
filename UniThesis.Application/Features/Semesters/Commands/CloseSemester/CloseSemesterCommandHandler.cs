using MediatR;
using UniThesis.Application.Common.Abstractions;
using UniThesis.Domain.Aggregates.SemesterAggregate;
using UniThesis.Domain.Common.Exceptions;
using UniThesis.Domain.Common.Interfaces;

namespace UniThesis.Application.Features.Semesters.Commands.CloseSemester;

/// <summary>
/// Handles the CloseSemesterCommand.
/// Loads the semester and transitions it from Active to Closed.
/// </summary>
public class CloseSemesterCommandHandler : ICommandHandler<CloseSemesterCommand>
{
    private readonly ISemesterRepository _semesterRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CloseSemesterCommandHandler(
        ISemesterRepository semesterRepository,
        IUnitOfWork unitOfWork)
    {
        _semesterRepository = semesterRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(CloseSemesterCommand request, CancellationToken cancellationToken)
    {
        var semester = await _semesterRepository.GetByIdAsync(request.SemesterId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Semester), request.SemesterId);

        semester.Close();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
