using CoreHackerNews.Dtos;
using CoreHackerNews.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CoreHackerNews.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsLogic _newsLogic;

        public NewsController(INewsLogic newsLogic)
        {
            _newsLogic = newsLogic ?? throw new ArgumentNullException(nameof(newsLogic));
        }

        [HttpGet("best-stories")]
        public async Task<ActionResult<IEnumerable<NewsItemDto>>> GetAsync()
        {
            try
            {
                var results = await _newsLogic.GetBestStoriesAsync();
                if (results == null || !results.Any())
                {
                    return NotFound();
                }

                return Ok(results.Select(r => new NewsItemDto(r)));
            }
            catch (HackerNewsClientException ex)
            {
                if (ex.StatusCode.HasValue)
                {
                    return StatusCode((int)ex.StatusCode.Value, ex.Message);
                }

                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
