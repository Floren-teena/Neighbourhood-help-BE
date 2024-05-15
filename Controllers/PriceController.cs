using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NeighbourhoodHelp.Core.IServices;
using NeighbourhoodHelp.Model.DTOs;
using NeighbourhoodHelp.Model.Entities;

namespace NeighbourhoodHelp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceController : ControllerBase
    {
        private readonly IPriceServices _priceService;

        public PriceController(IPriceServices priceService)
        {
            _priceService = priceService;
        }

        [HttpPost("agent/accept")]
        public async Task<IActionResult> AgentAcceptPrice(Guid errandId)
        {
            var result = await _priceService.AgentAcceptPrice(errandId);
            return Ok(new { message = result });
        }

        [HttpPost("agent/counter")]
        public async Task<IActionResult> AgentCounterPrice(PriceNegotiationDto request)
        {
            var result = await _priceService.AgentCounterPrice(request);
            return Ok(new { message = result });
        }

        [HttpPost("agent/decline")]
        public async Task<IActionResult> AgentDeclinePrice(Guid errandId)
        {
            var result = await _priceService.AgentDeclinePrice(errandId);
            return Ok(new { message = result });
        }

        [HttpPost("user/accept")]
        public async Task<IActionResult> UserAcceptPrice(Guid errandId)
        {
            var result = await _priceService.UserAcceptPrice(errandId);
            return Ok(new { message = result });
        }

        [HttpPost("user/counter")]
        public async Task<IActionResult> UserCounterPrice(PriceNegotiationDto request)
        {
            var result = await _priceService.UserCounterPrice(request);
            return Ok(new { message = result });
        }

        [HttpPost("user/decline")]
        public async Task<IActionResult> UserDeclinePrice(Guid errandId)
        {
            var result = await _priceService.UserDeclinePrice(errandId);
            return Ok(new { message = result });
        }
    }
}
