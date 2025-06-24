using System.Text.Json.Serialization;

namespace RegioMon.RegioJet
{
    [JsonSerializable(typeof(RegioJetListResponse))]
    [JsonSerializable(typeof(RegioJetRouteDetailResponse))]
    [JsonSerializable(typeof(RegioJetFreeSeatsRequest))]
    [JsonSerializable(typeof(RegioJetFreeSeatsResponse[]))]
    [JsonSourceGenerationOptions(
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    )]
    internal partial class RegioJetJsonSerializerContext : JsonSerializerContext
    {
    }
}
