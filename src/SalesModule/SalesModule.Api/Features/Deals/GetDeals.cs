using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using SalesModule.Contracts.Deals.Requests;
using SalesModule.Contracts.Deals.Responses;
using SalesModule.Infrastructure.Data.Queries;

namespace SalesModule.Api.Features.Deals;

public static class GetDeals
{
    public static async Task<Ok<PagedResponse<DealResponse>>> Handle([AsParameters] GetDealsRequest request, IDealsQueries queries)
    {
        return TypedResults.Ok(await queries.GetDeals(request)); 
    } 
}
