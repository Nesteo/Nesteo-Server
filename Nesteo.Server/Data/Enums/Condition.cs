using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nesteo.Server.Data.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Condition
    {
        Good = 0,
        NeedsRepair = 1,
        NeedsReplacement = 2
    }
}
