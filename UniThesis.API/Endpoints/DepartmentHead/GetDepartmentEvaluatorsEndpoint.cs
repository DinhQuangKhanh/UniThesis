using MediatR;
using UniThesis.API.Extensions;
using UniThesis.Application.Features.Departments.Queries.GetDepartmentEvaluators;
using UniThesis.Infrastructure.Authorization.Policies;
using static UniThesis.API.Extensions.ApiResponseExtensions;

namespace UniThesis.API.Endpoints.DepartmentHead;

public class GetDepartmentEvaluatorsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/department-head/evaluators", async (
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetDepartmentEvaluatorsQuery(), cancellationToken);
                return Ok(result);
            })
             .RequireAuthorization(PolicyNames.DepartmentHeadOfDepartment)
            .WithTags("DepartmentHead")
            .WithName("GetDepartmentEvaluators");
    }
}
