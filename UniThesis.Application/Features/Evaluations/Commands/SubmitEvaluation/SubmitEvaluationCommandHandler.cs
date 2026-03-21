using MediatR;
using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Common.Interfaces;
using UniThesis.Domain.Aggregates.EvaluationAggregate;
using UniThesis.Domain.Enums.Evaluation;
using IUnitOfWork = UniThesis.Domain.Common.Interfaces.IUnitOfWork;

namespace UniThesis.Application.Features.Evaluations.Commands.SubmitEvaluation;

public class SubmitEvaluationCommandHandler : ICommandHandler<SubmitEvaluationCommand>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProjectEvaluatorAssignmentRepository _assignmentRepository;

    public SubmitEvaluationCommandHandler(
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork,
        IProjectEvaluatorAssignmentRepository assignmentRepository)
    {
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _assignmentRepository = assignmentRepository;
    }

    public async Task<Unit> Handle(SubmitEvaluationCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated || _currentUser.UserId is null)
            throw new UnauthorizedAccessException("User is not authenticated.");

        var evaluatorId = _currentUser.UserId.Value;

        var assignment = await _assignmentRepository.GetActiveByProjectAndEvaluatorAsync(
            request.ProjectId, evaluatorId, cancellationToken)
            ?? throw new InvalidOperationException("Không tìm thấy phân công thẩm định cho đề tài này.");

        var result = (EvaluationResult)request.Result;
        assignment.SubmitEvaluation(result, request.Feedback);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
