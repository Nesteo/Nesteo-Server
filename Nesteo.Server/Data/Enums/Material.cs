using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nesteo.Server.Data.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Material
    {
        Other = 0,
        TreatedWood = 1,
        UntreatedWood = 2,
        WoodConcrete = 3
    }
}
