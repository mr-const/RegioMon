using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RegioMon.RegioJet
{
    public class RegioJetApi
    {
        private readonly ILogger<RegioJetApi> _logger;
        private readonly HttpClient _httpClient;

        private const string Endpoint = "https://brn-ybus-pubapi.sa.cz/";
        private const string Origin = "https://regiojet.com";
        private const string Referer = "https://regiojet.com/";
        private const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:139.0) Gecko/20100101 Firefox/139.0";

        public RegioJetApi(
            ILogger<RegioJetApi> logger,
            HttpClient httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;

            // Add default headers for all requests, just in case (works without them too)
            _httpClient.DefaultRequestHeaders.Add("Origin", Origin);
            _httpClient.DefaultRequestHeaders.Add("Referer", Referer);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);

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

        public async Task<RegioJetRouteDetailResponse?> GetRouteDetails(string routeId, long fromStationId, long toStationId)
        {
            // https://brn-ybus-pubapi.sa.cz/restapi/routes/7821682044/simple?fromStationId=372825000&routeId=7821682044&tariffs=REGULAR&toStationId=5990046024
            string url = Endpoint + $"restapi/routes/{routeId}/simple?fromStationId={fromStationId}&routeId={routeId}&tariffs=REGULAR&toStationId={toStationId}";

            _logger.LogDebug("Requesting route details for routeId: {RouteId}", routeId);
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to retrieve route details: {response.ReasonPhrase}");
            }

            // Use the RegioJetJsonSerializerContext for deserialization
            return JsonSerializer.Deserialize(response.Content.ReadAsStream(), RegioJetJsonSerializerContext.Default.RegioJetRouteDetailResponse);
        }

        public async Task<RegioJetFreeSeatsResponse[]?> GetFreeSeats(long sectionId, long fromStationId, long toStationId, string seatClass)
        {
            string url = Endpoint + "restapi/routes/freeSeats";

            // Create the request body
            var requestBody = new RegioJetFreeSeatsRequest
            {
                Sections = new List<SectionRequest>
                {
                    new SectionRequest
                    {
                        SectionId = sectionId,
                        FromStationId = fromStationId,
                        ToStationId = toStationId
                    }
                },
                SeatClass = seatClass,
                Tariffs = ["REGULAR"]
            };

            _logger.LogDebug("Requesting free seats for sectionId: {SectionId}, seatClass: {SeatClass}", sectionId, seatClass);

            // Serialize using JsonSerializerOptions since the generated context isn't working well
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
#pragma warning disable IL2026, IL3050
                // Disable warnings for AOT and code trimming
            };

            string jsonRequestBody = JsonSerializer.Serialize(requestBody, options);
            var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

            // Make the POST request
            var response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to retrieve free seats for train {Train}: {StatusCode} - {ReasonPhrase}", sectionId, response.StatusCode, response.ReasonPhrase);
                return [];
            }

            // Deserialize the response
            return JsonSerializer.Deserialize<RegioJetFreeSeatsResponse[]>(response.Content.ReadAsStream(), options);
#pragma warning restore IL2026, IL3050
        }
    }
}
