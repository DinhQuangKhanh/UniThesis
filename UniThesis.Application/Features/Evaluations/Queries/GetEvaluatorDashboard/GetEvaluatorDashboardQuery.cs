using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.Evaluations.DTOs;

namespace UniThesis.Application.Features.Evaluations.Queries.GetEvaluatorDashboard;

public record GetEvaluatorDashboardQuery() : IQuery<EvaluatorDashboardDto>;
