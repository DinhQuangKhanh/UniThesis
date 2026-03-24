using MediatR;
using UniThesis.API.Extensions;
using UniThesis.Application.Features.Departments.Queries.GetDepartmentProjects;
using static UniThesis.API.Extensions.ApiResponseExtensions;

namespace UniThesis.API.Endpoints.DepartmentHead;

public class GetDepartmentProjectsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/department-head/projects", async (
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetDepartmentProjectsQuery(), cancellationToken);
                return Ok(result);
            })
            .RequireAuthorization("RequireDepartmentHead")
            .WithTags("DepartmentHead")
            .WithName("GetDepartmentProjects");
    }
}
