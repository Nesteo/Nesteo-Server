using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nesteo.Server.Data.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Condition
    {
        Good,
        NeedsRepair,
        NeedsReplacement
    }
}
