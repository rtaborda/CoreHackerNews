using CoreHackerNews.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var results = await _newsLogic.GetBestStoriesAsync();
            if (results == null || !results.Any())
            {
                return NotFound();
            }

            return Ok(results.Select(r => new NewsItemDto(r)));
        }
    }
}
