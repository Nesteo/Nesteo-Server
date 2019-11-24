using System.Linq;
using System.Reflection;
using System.Text;

namespace Nesteo.Server.DataImport
{
    public static class Utils
    {
        public static string SerializeObjectProperties(object @object)
        {
            PropertyInfo[] properties = @object.GetType().GetProperties();
            return string.Join(", ",
                               properties.Select(property => {
                                   string propertyName = property.Name;
                                   string value = property.GetValue(@object)?.ToString();
                                   if (string.IsNullOrWhiteSpace(value))
                                       value = "null";
                                   return $"{propertyName}={value}";
                               }));
        }
    }
}
