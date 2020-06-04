using bridges_structures_service.Models;
using bridges_structures_service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using System.Threading.Tasks;

namespace bridges_structures_service.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[Controller]")]
    [ApiController]
    [TokenAuthentication]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBridgesStructuresService _bridgesStructuresService;

        public HomeController(ILogger<HomeController> logger, IBridgesStructuresService bridgesStructuresService)
        {
            _logger = logger;
            _bridgesStructuresService = bridgesStructuresService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]BridgesStructuresReport bridgesStructuresReport)
        {
            string result = await _bridgesStructuresService.CreateCase(bridgesStructuresReport);

            return Ok(result);
        }
    }
}