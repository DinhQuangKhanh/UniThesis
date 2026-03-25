using UniThesis.Application.Common.Abstractions;

namespace UniThesis.Application.Features.DirectRegistration.Commands.CreateDirectTopic;

public record CreateDirectTopicCommand(
    string NameVi,
    string NameEn,
    string NameAbbr,
    string Description,
    string Objectives,
    string? Scope,
    string? Technologies,
    string? ExpectedResults,
    Guid MentorId,
    Guid GroupId,
    int MajorId,
    int MaxStudents
) : ICommand<Guid>;
