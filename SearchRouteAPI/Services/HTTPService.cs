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

        public static async Task<string> GetRoutes(string alias, CancellationToken ct)
        {
            string url = $"https://homework.appulate.com/api/Route/outgoing?airport={alias}";
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                var response = await client.SendAsync(request, ct).ConfigureAwait(false);
                var routes = await response.Content.ReadAsStringAsync();
                return routes;
            }
        }

        public static async Task<List<Airport>> GetAirports(string pattern, CancellationToken ct)
        {
            string url = $"https://homework.appulate.com/api/Airport/search?pattern={pattern}";
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                var response = await client.SendAsync(request, ct).ConfigureAwait(false);
                var airoports = JsonConvert.DeserializeObject<List<Airport>>(await response.Content.ReadAsStringAsync());
                return airoports;
            }
        }

        public static async Task<string> GetData(string endpoint)
        {
            Uri uri = new Uri($"https://homework.appulate.com/api/{endpoint}");
            var maxRetryAttempts = 3;
            var pauseBetweenFailures = TimeSpan.FromSeconds(3);
            string content = "";
                
            try
            {
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
                { 
                    await RetryHelper.RetryOnExceptionAsync<WebException>
                        (maxRetryAttempts, pauseBetweenFailures, async () => {
                            var response = await client.SendAsync(request);
                            content = await response.Content.ReadAsStringAsync();
                        });
                    
                    return content;
                }
                //using (WebClient webClient = new WebClient())
                //{
                //    await RetryHelper.RetryOnExceptionAsync<WebException>
                //        (maxRetryAttempts, pauseBetweenFailures, async () => {
                //            resultData = await webClient.DownloadStringTaskAsync(uri);
                //        });

                //    return resultData;
                //}
            }
            catch (WebException wex)
            {
                return wex.Message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
