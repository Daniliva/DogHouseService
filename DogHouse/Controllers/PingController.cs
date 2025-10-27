using DogHouse.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DogHouse.Controllers
{
    [ApiController]
    [Route("/")]
    public class PingController : ControllerBase
    {
        private readonly IDogService _svc;
        public PingController(IDogService svc) => _svc = svc;


        [HttpGet("ping")]
        public async Task<IActionResult> Ping() => Ok(await _svc.PingAsync());
    }
}
