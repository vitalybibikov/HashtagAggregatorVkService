using System.Threading.Tasks;
using HashtagAggregatorVk.Contracts.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HashtagAggregatorVk.Service.Controllers
{
    [Route("api/[controller]")]
   // [Authorize]
    public class HeartBeatController : Controller
    {
        private readonly IBackgroundServiceWorker worker;

        public HeartBeatController(IBackgroundServiceWorker worker)
        {
            this.worker = worker;
        }

        [HttpGet("ping")]
        [AllowAnonymous]
        public IActionResult Ping()
        {
            return Ok();
        }

        [HttpGet("start/{hashtag:required}")]
        public async Task<IActionResult> Start(string hashtag)
        {
            var result = await worker.Start(hashtag);
            return Ok(result);
        }

        [HttpGet("stop/{hashtag:required}")]
        public IActionResult Stop(string hashtag)
        {
            var result = worker.Stop(hashtag);
            return Ok(result);
        }
    }
}