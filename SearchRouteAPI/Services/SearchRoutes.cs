using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

using SearchRouteAPI.Models;

namespace SearchRouteAPI.Services
{
    public class SearchRoutes
    {
        private CancellationTokenSource cts;

        private string srcAirport;
        private string destAirport;
        private List<string> visited;

        private string result_string;
        
        public SearchRoutes(string _srcAirport, string _destAirport)
        {
            srcAirport = _srcAirport;
            destAirport = _destAirport;
            cts = new CancellationTokenSource();
            visited = new List<string>();

            result_string = _srcAirport;
        }
        
        public async Task<string> FindRoutes(string srcAirport, string resultRoutes, CancellationToken ct)
        {
            //add srcAirport to checked list
            visited.Add(srcAirport);

            try
            {
                var results = JsonConvert.DeserializeObject<List<Route>>(await HTTPService.GetRoutes(srcAirport, cts.Token));
                
                foreach (var r in results)
                {
                    if (r.destAirport == destAirport)
                    {
                        cts.Cancel();
                        result_string = String.Concat(result_string, resultRoutes, " -> ", r.destAirport);
                        return result_string;
                    }
                }

                
                var srcList = new List<string>();
                results.ForEach(r => {
                    if (!visited.Contains(r.destAirport))
                        srcList.Add(r.destAirport);
                        
                });
                
                var tasks = srcList.Select(s => FindRoutes(s, String.Concat(resultRoutes + " -> ", s), ct));

                var sss = await Task.WhenAny(tasks);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result_string;
        }
        
    }
}
