using MediatR;
using UniThesis.API.Extensions;
using UniThesis.Application.Features.Departments.Queries.GetDepartmentProjects;
using UniThesis.Infrastructure.Authorization.Policies;
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
             .RequireAuthorization(PolicyNames.DepartmentHeadOfDepartment)
            .WithTags("DepartmentHead")
            .WithName("GetDepartmentProjects");
    }
}
