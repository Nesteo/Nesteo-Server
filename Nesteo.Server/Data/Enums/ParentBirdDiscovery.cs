using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nesteo.Server.Data.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ParentBirdDiscovery
    {
        None = 0,
        NotRinged = 1,
        AlreadyRinged = 2,
        NewlyRinged = 3
    }
}
