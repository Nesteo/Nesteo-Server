using CsvHelper.Configuration.Attributes;

namespace Nesteo.Server.DataImport.RecordModels
{
    public class NestingBoxRecord
    {
        [Name("nistkasten-nummer")]
        public string Id { get; set; }

        [Name("nummer-fremd")]
        public string ForeignId { get; set; }

        [Name("ort")]
        public string RegionCityName { get; set; }

        [Name("ort detailliert")]
        public string RegionDetailedName { get; set; }

        [Name("utm-hoch")]
        public string UtmNorth { get; set; }

        [Name("utm-rechts")]
        public string UtmEast { get; set; }

        [Name("aufhäng-datum")]
        public string HangUpDate { get; set; }

        [Name("eigentümer")]
        public string OwnerName { get; set; }

        [Name("material")]
        public string Material { get; set; }

        [Name("loch")]
        public string HoleSize { get; set; }

        [Name("bemerkungen")]
        public string Comments { get; set; }

        [Name("daten aktualisiert")]
        public string DataUpdateDate { get; set; }
    }
}
