using System;
using System.Collections.Generic;
using System.Linq;

namespace Nesteo.Server.Utils
{
    public static class CsvSerializationHelper
    {
        // See: https://tools.ietf.org/html/rfc4180#section-2
        public static string SerializeCsvRow(params object[] fields)
        {
            if (fields == null)
                throw new ArgumentNullException(nameof(fields));
            
            // Convert field values to strings
            IEnumerable<string> encodedFieldValues = fields.Select(value => value?.ToString() ?? string.Empty);

            // Escape double-quotes in value
            encodedFieldValues = encodedFieldValues.Select(value => value.Replace("\"", "\"\""));

            // Enclose fields containing reserved characters with double-quotes
            encodedFieldValues = encodedFieldValues.Select(value => {
                if (value.Contains("\r\n") || value.Contains("\"") || value.Contains(","))
                    value = $"\"{value}\"";
                return value;
            });

            // Join fields to a single CSV row
            return string.Join(",", encodedFieldValues);
        }
    }
}
