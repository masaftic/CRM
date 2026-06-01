using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using SalesModule.Contracts.Pipelines.Requests;
using Shared.Web.Extensions;

namespace SalesModule.Api.Features.Pipelines;

public static class PipelineEndpoints
{
    public static void MapPipelineEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/", CreatePipeline.Handle)
            .WithValidation<CreatePipelineRequest>();
        routes.MapGet("/{pipelineId}", GetPipeline.Handle);
        routes.MapPut("/{pipelineId}", UpdatePipeline.Handle);
        routes.MapDelete("/{pipelineId}", DeletePipeline.Handle);

        // Stage endpoints
        routes.MapPost("/{pipelineId}/stages", AddStage.Handle);
        routes.MapDelete("/{pipelineId}/stages/{stageId}", RemoveStage.Handle);
        routes.MapPut("/{pipelineId}/stages/{stageId}", UpdateStage.Handle);
        routes.MapPut("/{pipelineId}/stages/{stageId}/move", MoveStage.Handle);
    }
}
