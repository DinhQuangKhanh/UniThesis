using UniThesis.Application.Common.Abstractions;
using UniThesis.Application.Features.Evaluations.DTOs;

namespace UniThesis.Application.Features.Evaluations.Queries.GetEvaluatorFilterOptions;

public record GetEvaluatorFilterOptionsQuery() : IQuery<EvaluatorFilterOptionsDto>;
