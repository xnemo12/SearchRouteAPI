using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using SearchRouteAPI.Models;
using SearchRouteAPI.Services;

namespace SearchRouteAPI.Controllers
{
    [Produces("application/json")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [Route("api/[controller]")]
    [ApiController]
    public class RoutesController : ControllerBase
    {
        [HttpGet("{srcAirport}/{destAirport}")]
        public async Task<ActionResult<string>> GetAsync(string srcAirport, string destAirport)
        {
            SearchRoutes sr = new SearchRoutes(srcAirport, destAirport);

            //check airport avialable
            if (!await sr.IsAirportAvialable(srcAirport))
                return BadRequest(new { message = $"{srcAirport} is not avialable" });

            if (!await sr.IsAirportAvialable(destAirport))
                return BadRequest(new { message = $"{destAirport} is not avialable" });

            var result = await sr.FindRoutes(srcAirport, String.Empty);
            return Ok(new { result = result });
        }

    }
}
