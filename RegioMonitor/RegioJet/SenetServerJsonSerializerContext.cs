using System.Text.Json.Serialization;

namespace RegioMon.RegioJet
{
    [JsonSerializable(typeof(RegioJetListResponse))]
    [JsonSourceGenerationOptions(
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    )]
    internal partial class RegioJetJsonSerializerContext : JsonSerializerContext
    {
    }
}
