using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nesteo.Server.Data.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum HoleSize
    {
        Other = 0,
        Small = 1,
        Medium = 2,
        Large = 3,
        VeryLarge = 4,
        Oval = 5,
        OpenFronted = 6
    }
}
