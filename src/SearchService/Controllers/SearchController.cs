using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Model;
using SearchService.SearchHelper;
using ZstdSharp.Unsafe;

namespace SearchService.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Item>>> SearchItems([FromQuery] SearchParams searchParams, int pageNumber = 1, int pageSize = 4)
    {
        var query = DB.PagedSearch<Item, Item>();

        query.Sort(x => x.Ascending(a => a.Make));

        if (!string.IsNullOrEmpty(searchParams.SearchTerm))
        {
            query.Match(Search.Full, searchParams.SearchTerm).SortByTextScore();
        }

        query.PageNumber(searchParams.PageNumber);
        query.PageSize(searchParams.PageSize);

        query = searchParams.OrderBy switch
        {
            "make" => query.Sort(s => s.Ascending(x => x.Make)),
            "new" => query.Sort(s => s.Descending(x => x.CreateAt)),
            _ => query.Sort(s => s.Ascending(x => x.AuctionEnd)),
        };

        query = searchParams.FilterBy switch
        {
            "finished" => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
            "endingsoon" => query.Match(x => x.AuctionEnd < DateTime.Now.AddHours(6) &&
                            x.AuctionEnd > DateTime.UtcNow),
            _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow)
        };


        if (!string.IsNullOrEmpty(searchParams.Seller))
        {
            query = query.Match(x => x.Seller == searchParams.Seller);
        }

        if (!string.IsNullOrEmpty(searchParams.Winner))
        {
            query = query.Match(x => x.Winner == searchParams.Winner);
        }

        var result = await query.ExecuteAsync();

        return Ok(new
        {
            results = result.Results,
            PageCount = result.PageCount,
            TotalCount = result.TotalCount,
        });
    }
}
