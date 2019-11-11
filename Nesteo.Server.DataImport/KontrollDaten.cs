using CsvHelper.Configuration.Attributes;

namespace Nesteo.Server.DataImport
{
    public class Kontrolldaten
    {
        [Index(0)]
        public string NistkastenNummer { get; set; }

        [Index(1)]
        public string Datum { get; set; }

        [Index(2)]
        public string ZustandKasten { get; set; }

        [Index(3)]
        public string Gereinigt { get; set; }

        [Index (4)]
        public string Besetzt { get; set; }

        [Index(5)]
        public string AnzahlEier { get; set; }

        [Index(6)]
        public string AnzahlJungvogel { get; set; }

        [Index(7)]
        public string AlterJungvogel { get; set; }

        [Index(8)]
        public string Vogelart { get; set; }

        [Index(9)]
        public string Berignt { get; set; }

        [Index(10)]
        public string Bemerkungen { get; set; }
    }

}
