using FiboForm.Common;
using FiboForm.Common.Components.Definition;
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
        private readonly IEnvironmentSettings _environmentSettings;

        public FiboFormRepository(IHttpClientFactory httpClientFactory, ILogger<FiboFormRepository> logger, IEnvironmentSettings environmentSettings)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _environmentSettings = environmentSettings;
        }

        public async Task<FiboModel> GetPayload()
        {
            var baseUrl = _environmentSettings.GetEnvironmentVariable("API_BASE_URL");
            var payloadUrl = _environmentSettings.GetEnvironmentVariable("FIBO_PAYLOAD_URL");

            using (var client = _httpClientFactory.CreateClient())
            {
                var request = new HttpRequestMessage
                {
                    Content = null,
                    Method = HttpMethod.Get,
                    RequestUri = new System.Uri(baseUrl + payloadUrl)
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
            var baseUrl = _environmentSettings.GetEnvironmentVariable("API_BASE_URL");
            var searchUrl = string.Format(_environmentSettings.GetEnvironmentVariable("FIBO_SEARCH_URL"), index);

            using (var client = _httpClientFactory.CreateClient())
            {
                var request = new HttpRequestMessage
                {
                    Content = null,
                    Method = HttpMethod.Post,
                    RequestUri = new System.Uri(baseUrl + searchUrl)
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
