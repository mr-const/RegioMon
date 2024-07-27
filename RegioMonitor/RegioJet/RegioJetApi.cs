using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RegioMon.RegioJet
{
    public class RegioJetApi
    {
        private readonly ILogger<RegioJetApi> _logger;
        private readonly HttpClient _httpClient;

        private const string Endpoint = "https://brn-ybus-pubapi.sa.cz/";

        public RegioJetApi(
            ILogger<RegioJetApi> logger,
            HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<RegioJetListResponse?> SimpleRouteSearch(string departureDate, long fromLocationId, long toLocationId)
        {
            // https://brn-ybus-pubapi.sa.cz/restapi/routes/search/simple?tariffs=REGULAR&toLocationType=CITY&toLocationId=10202003&fromLocationType=CITY&fromLocationId=5990055004&departureDate=2024-08-07&fromLocationName=&toLocationName=
            string url = Endpoint + $"restapi/routes/search/simple?tariffs=REGULAR&toLocationType=CITY&fromLocationType=CITY&fromLocationName=&toLocationName=&toLocationId={toLocationId}&fromLocationId={fromLocationId}&departureDate={departureDate}";


            _logger.LogDebug("Requesting train list on {Date}", departureDate);
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to retrieve simple routes: {response.ReasonPhrase}");
            }

            return JsonSerializer.Deserialize(response.Content.ReadAsStream(), RegioJetJsonSerializerContext.Default.RegioJetListResponse);
        }
    }
}
