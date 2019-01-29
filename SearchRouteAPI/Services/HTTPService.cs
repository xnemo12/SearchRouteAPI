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

        public static async Task<T> SendData<T>(string url)
        {

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                var response = await client.SendAsync(request).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
            }
        }

        public static async Task<List<Airline>> GetAirlines(string alias)
        {
            string url = $"https://homework.appulate.com/api/Airline/{alias}";
            return await SendData<List<Airline>>(url);
        }

        public static async Task<List<Airport>> GetAirports(string pattern)
        {
            string url = $"https://homework.appulate.com/api/Airport/search?pattern={pattern}";
            return await SendData<List<Airport>>(url);            
        }

        public static async Task<List<Route>> GetRoutes(string alias, CancellationToken ct)
        {
            Uri uri = new Uri($"https://homework.appulate.com/api/Route/outgoing?airport={alias}");
            var maxRetryAttempts = 3;
            var pauseBetweenFailures = TimeSpan.FromSeconds(3);
            var content = new List<Route>();

            try
            {
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
                {
                    await RetryHelper.RetryOnExceptionAsync<WebException>
                        (maxRetryAttempts, pauseBetweenFailures, async () =>
                        {
                            var response = await client.SendAsync(request, ct).ConfigureAwait(false);
                            content = JsonConvert.DeserializeObject<List<Route>>(await response.Content.ReadAsStringAsync());
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
