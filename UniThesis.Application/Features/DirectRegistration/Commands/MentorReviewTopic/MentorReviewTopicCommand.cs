using UniThesis.Application.Common.Abstractions;

namespace UniThesis.Application.Features.DirectRegistration.Commands.MentorReviewTopic;

public record MentorReviewTopicCommand(
    Guid ProjectId,
    string Action,
    string? Feedback
) : ICommand;
