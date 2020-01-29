using CsvHelper.Configuration.Attributes;

namespace Nesteo.Server.DataImport.RecordModels
{
    public class InspectionRecord
    {
        [Name("nistkasten-nummer")]
        public string NestingBoxId { get; set; }

        [Name("datum")]
        public string Date { get; set; }

        [Name("zustand kasten")]
        public string Condition { get; set; }

        [Name("gereinigt")]
        public string HasBeenCleaned { get; set; }

        [Name("besetzt")]
        public string Occupied { get; set; }

        [Name("anzahl eier")]
        public string EggCount { get; set; }

        [Name("anzahl jungvögel")]
        public string ChickCount { get; set; }

        [Name("alter jungvögel")]
        public string ChickAges { get; set; }

        [Name("vogelart")]
        public string SpeciesName { get; set; }

        [Name("beringt")]
        public string RingedCount { get; set; }

        [Name("bemerkungen")]
        public string Comments { get; set; }
    }
}
