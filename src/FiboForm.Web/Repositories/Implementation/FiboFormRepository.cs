using FiboForm.Common;
using FiboForm.Web.Models;
using FiboForm.Web.Repositories.Definition;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FiboForm.Web.Repositories.Implementation
{
    public class FiboFormRepository : IFiboFormRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<FiboFormRepository> _logger;

        public FiboFormRepository(IHttpClientFactory httpClientFactory, ILogger<FiboFormRepository> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<FiboModel> GetPayload()
        
        {
            var url = EnvironmentSettings.GetEnvironmentVariable("API_URL") + "/fibo";

            using (var client = _httpClientFactory.CreateClient())
            {
                var request = new HttpRequestMessage
                {
                    Content = null,
                    Method = HttpMethod.Get,
                    RequestUri = new System.Uri(url)
                };

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<Payload>(jsonResult);

                    return MapModel(result);

                }
                else
                    return new FiboModel { ErrorMessage = $"Get Fibo Values Failed, HttpResponse: {response.StatusCode.ToString()}" };
            }
        }

        public async Task<FiboModel> SearchIndex(int index)
        {
            var url = EnvironmentSettings.GetEnvironmentVariable("API_URL") + $"/fibo/{index}";

            using (var client = _httpClientFactory.CreateClient())
            {
                var request = new HttpRequestMessage
                {
                    Content = null,
                    Method = HttpMethod.Post,
                    RequestUri = new System.Uri(url)
                };

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<Payload>(jsonResult);

                    return MapModel(result);

                }
                else 
                    return new FiboModel { ErrorMessage = $"Search Fibo Index Failed from API, HttpResponse: {response.StatusCode.ToString()}" };
            }
        }

        private FiboModel MapModel(Payload result)
        {
            return new FiboModel
            {
                VisitedIndexes = result.VisitedIndexes,
                VisitedValues = result.VisitedValues.Select(e => new VisitedValue { Index = e.Index, Value = e.Value }).ToList()
            };
        }
    }
}
