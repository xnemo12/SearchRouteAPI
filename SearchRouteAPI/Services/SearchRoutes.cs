using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using SearchRouteAPI.Models;

namespace SearchRouteAPI.Services
{
    public class SearchRoutes
    {
        private CancellationTokenSource cts;

        private string _srcAirport;
        private string _destAirport;

        //Search API visited airports list
        private List<string> visited;

        private string resultString;
        

        public SearchRoutes(string srcAirport, string destAirport)
        {
            _srcAirport = srcAirport;
            _destAirport = destAirport;
            cts = new CancellationTokenSource();
            visited = new List<string>();

            resultString = _srcAirport;
        }
        
        public async Task<string> FindRoutes(string _srcAirport, string resultRoutes)
        {
            //add srcAirport to checked list
            visited.Add(_srcAirport);

            try
            {
                //get results about destAirports
                List<Route> results = await HTTPService.GetRoutes(_srcAirport, cts.Token);
                
                if (results.Count > 0)
                {
                    //check results with searchable src and dest airports, is find - cancel any requests
                    foreach (var r in results)
                    {
                        //check airline is active
                        bool airlineActive = await IsAirlineActive(r.Airline);
                        
                        if (r.DestAirport == _destAirport && airlineActive)
                        {
                            cts.Cancel();
                            resultString = String.Concat(resultString, resultRoutes, " -> ", r.DestAirport);
                            return resultString;
                        }
                    }

                    //create new list src airports, compare with visited src airports
                    var srcList = new List<string>();
                    results.ForEach(r => {
                        if (!visited.Contains(r.DestAirport))
                            srcList.Add(r.DestAirport);
                    });

                    //new tasks list and start parallel all (recursive)
                    var tasks = srcList.Select(s => FindRoutes(s, String.Concat(resultRoutes + " -> ", s)));
                    await Task.WhenAll(tasks);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return resultString;
        }

        public async Task<bool> IsAirportAvialable(string airport)
        {
            var airports = await HTTPService.GetAirports(airport); 
            foreach (var a in airports)
            {
                if(a.Alias == airport)
                    return true;
            }
            return false;
        }

        public async Task<bool> IsAirlineActive(string alias)
        {
            var airlines = await HTTPService.GetAirlines(alias);
            foreach (var a in airlines)
            {
                if (a.Active)
                    return true;
            }
            return false;
        }


    }
}
