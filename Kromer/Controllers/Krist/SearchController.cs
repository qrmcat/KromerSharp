using Humanizer;
using Kromer.Models.Api.Krist.Lookup;
using Kromer.Models.Api.Krist.Transaction;
using Kromer.Models.Exceptions;
using Kromer.Services;
using Kromer.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Kromer.Controllers.Krist;

[Route("api/krist/search")]
[ApiController]
public class SearchController(SearchService searchService) : ControllerBase
{
    [HttpGet("")]
    public async Task<KristSearchResult> Search([FromQuery(Name = "q")] string? query)
    {
        query = SearchValidation.ValidateQuery(query);

        return await searchService.SearchAsync(query);
    }

    [HttpGet("extended")]
    public async Task<KristExtendedSearchResult> SearchExtended([FromQuery(Name = "q")] string? query)
    {
        query = SearchValidation.ValidateQuery(query);

        return await searchService.SearchExtendedAsync(query);
    }

    // https://tenor.com/en-GB/view/arriba-gif-12324188786972117417
    [HttpGet("extended/results/transactions/{type}")]
    public async Task<KristResultTransactions> SearchExtendedResultsTransactions(string type,
        [FromQuery(Name = "q")] string? query,
        [FromQuery] string orderBy = "id",
        [FromQuery] string order = "ASC",
        [FromQuery] bool includeMined = false,
        [FromQuery] int limit = 50,
        [FromQuery] int offset = 0)
    {
        query = SearchValidation.ValidateQuery(query);

        limit = Math.Clamp(limit, 1, 1000);

        if (!Enum.TryParse<TransactionSearchType>(type.Pascalize(), out var transactionType))
        {
            throw new KristParameterException("type");
        }

        if (!Enum.TryParse<TransactionOrderByParameter>(orderBy.Pascalize(), out var orderByParameter))
        {
            throw new KristParameterException("orderBy");
        }

        if (!Enum.TryParse<OrderParameter>(order.ToLowerInvariant().Pascalize(), out var orderParameter))
        {
            throw new KristParameterException("order");
        }

        return await searchService.SearchExtendedResultAsync(transactionType, query, orderByParameter, orderParameter,
            includeMined, limit, offset);
    }
}