using Newtonsoft.Json;
using SearchRouteAPI.Helpers;
using SearchRouteAPI.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SearchRouteAPI.Services
{
    public static class HTTPService
    {

        public static async Task<List<Route>> GetRoutes(string alias, CancellationToken ct)
        {
            return JsonConvert.DeserializeObject<List<Route>>(await GetData($"Route/outgoing?airport={alias}", ct));
        }

        public static async Task<List<Airport>> GetAirports(string pattern, CancellationToken ct)
        {
            return JsonConvert.DeserializeObject<List<Airport>>(await GetData($"Airport/search?pattern={pattern}", ct));
        }

        public static async Task<List<Airline>> GetAirlines(string alias, CancellationToken ct)
        {
            return JsonConvert.DeserializeObject<List<Airline>>(await GetData($"Airline/{alias}", ct));
        }
        

        public static async Task<string> GetData(string endpoint, CancellationToken ct)
        {
            Uri uri = new Uri($"https://homework.appulate.com/api/{endpoint}");
            var maxRetryAttempts = 3;
            var pauseBetweenFailures = TimeSpan.FromSeconds(3);
            var content = String.Empty;
                
            try
            {
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
                {
                    await RetryHelper.RetryOnExceptionAsync<WebException>
                        (maxRetryAttempts, pauseBetweenFailures, async () =>
                        {
                            var response = await client.SendAsync(request, ct).ConfigureAwait(false);
                            content = await response.Content.ReadAsStringAsync();
                        });
                }
            }
            catch (WebException wex)
            {
                Console.WriteLine(wex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return content;
        }
    }
}
