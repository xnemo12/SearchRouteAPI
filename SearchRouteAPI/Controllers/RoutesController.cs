using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using SearchRouteAPI.Models;
using SearchRouteAPI.Services;

namespace SearchRouteAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class RoutesController : ControllerBase
    {
        [HttpGet("{srcAirport}/{destAirport}")]
        public async Task<string> Get(string srcAirport, string destAirport)
        {
            SearchRoutes sr = new SearchRoutes(srcAirport, destAirport);

            //check airport avialable
            if (!await sr.IsAirportAvialable(srcAirport))
                return $"{srcAirport} is not avialable";

            if (!await sr.IsAirportAvialable(destAirport))
                return $"{destAirport} is not avialable";

            var result = await sr.FindRoutes(srcAirport, String.Empty);
            return result;
        }

    }
}
