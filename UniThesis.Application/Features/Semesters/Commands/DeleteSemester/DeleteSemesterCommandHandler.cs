using UniThesis.Application.Common.Abstractions;
using MediatR;
using UniThesis.Domain.Aggregates.SemesterAggregate;
using UniThesis.Domain.Common.Exceptions;
using UniThesis.Domain.Common.Interfaces;
using UniThesis.Domain.Enums.Semester;

namespace UniThesis.Application.Features.Semesters.Commands.DeleteSemester;

/// <summary>
/// Handles the DeleteSemesterCommand to remove an upcoming semester.
/// </summary>
public class DeleteSemesterCommandHandler : ICommandHandler<DeleteSemesterCommand>
{
    private readonly ISemesterRepository _semesterRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSemesterCommandHandler(
        ISemesterRepository semesterRepository,
        IUnitOfWork unitOfWork)
    {
        _semesterRepository = semesterRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteSemesterCommand request, CancellationToken cancellationToken)
    {
        var semester = await _semesterRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Semester), request.Id);

        if (semester.Status != SemesterStatus.Upcoming)
        {
            throw new BusinessRuleValidationException("Only upcoming semesters can be deleted.");
        }

        _semesterRepository.Remove(semester);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
