using CsvHelper.Configuration.Attributes;

namespace Nesteo.Server.DataImport
{
    public class Stammdaten
    {
        [Index(0)]
        public string NistkastenNummer { get; set; }

        [Index(1)]
        public string NummerFremd { get; set; }

        [Index(2)]
        public string Ort { get; set; }

        [Index(3)]
        public string OrtDetailliert { get; set; }

        [Index(4)]
        public string UTMHoch { get; set; }

        [Index(5)]
        public string UTMRechts { get; set; }

        [Index(6)]
        public string AufhangDatum { get; set; }

        [Index(7)]
        public string Eigentumer{ get; set; }

        [Index(8)]
        public string Material { get; set; }

        [Index(9)]
        public string Loch { get; set; }

        [Index(10)]
        public string Bemerkungen { get; set; }

        [Index(11)]
        public string AktualisiertDatum { get; set; }
    }
}
