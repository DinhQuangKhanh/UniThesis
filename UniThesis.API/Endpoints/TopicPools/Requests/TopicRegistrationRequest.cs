namespace UniThesis.API.Endpoints.TopicPools.Requests;

public sealed record TopicRegistrationRequest(Guid ProjectId, Guid GroupId, string? Note);
