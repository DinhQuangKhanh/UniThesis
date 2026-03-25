using UniThesis.Application.Common.Abstractions;

namespace UniThesis.Application.Features.DirectRegistration.Commands.SubmitToMentor;

public record SubmitToMentorCommand(Guid ProjectId, Guid GroupId) : ICommand;
