using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;

namespace SearchService;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Item>>> SearchItems([FromQuery] SearchParams searchParams)
    {
        // pagedSearch can be return a Pagination Type
        var query = DB.PagedSearch<Item, Item>();

        // Check the search term is null or empty
        if (!string.IsNullOrEmpty(searchParams.searchTerm))
        {
            // search the item based on search term
            query.Match(Search.Full, searchParams.searchTerm).SortByTextScore();
        }

        // set up the order
        query = searchParams.OrderBy switch
        {
            "make" => query.Sort(x => x.Ascending(a => a.Make)),
            "new" => query.Sort(x => x.Descending(a => a.CreateAt)),
            _ => query.Sort(x => x.Ascending(a => a.AuctionEnd))
        };

        // set up the filter  
        query = searchParams.FilterBy switch
        {
            "finished" => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
            "endingSoon" => query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(6)
            && x.AuctionEnd > DateTime.UtcNow),
            _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow)
        };

        //check if seller is null or empty
        if (!string.IsNullOrEmpty(searchParams.Seller))
        {
            // get the result that match with seller
            query.Match(x => x.Seller == searchParams.Seller);
        }

        //check if winner is null or empty
        if (!string.IsNullOrEmpty(searchParams.Winner))
        {
            // get the result that match with winner
            query.Match(x => x.Winner == searchParams.Winner);
        }


        query.PageNumber(searchParams.pageNumber);
        query.PageSize(searchParams.pageSize);

        var result = await query.ExecuteAsync();

        return Ok(new
        {
            result = result.Results,
            pageCount = result.PageCount,
            totalCount = result.TotalCount
        });
    }
}
