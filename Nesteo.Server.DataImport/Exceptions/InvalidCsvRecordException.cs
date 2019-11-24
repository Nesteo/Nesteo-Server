using System;

namespace Nesteo.Server.DataImport.Exceptions
{
    public class InvalidCsvRecordException : ImportException
    {
        public InvalidCsvRecordException() { }

        public InvalidCsvRecordException(string message) : base(message) { }

        public InvalidCsvRecordException(string message, Exception innerException) : base(message, innerException) { }
    }
}
