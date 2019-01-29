using System;
using System.Collections.Generic;
using System.Threading;
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
            CancellationTokenSource cts = new CancellationTokenSource();

            SearchRoutes sr = new SearchRoutes(srcAirport, destAirport);

            var routes = await sr.FindRoutes(srcAirport, "", cts.Token);

            cts.Cancel();
        
            return routes;
        }

    }
}
