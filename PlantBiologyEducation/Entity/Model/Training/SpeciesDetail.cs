using System.Text.Json.Serialization;

namespace PlantBiologyEducation.Entity.Model.Training
{
    public class SpeciesDetail
    {
        [JsonPropertyName("CommonName")]
        public string CommonName { get; set; }
        [JsonPropertyName("ScientificName")]
        public string ScientificName { get; set; }
        [JsonPropertyName("SpecieType")]
        public string SpecieType { get; set; }
        [JsonPropertyName("Description")]
        public string Description { get; set; }
        [JsonPropertyName("Habitat")]
        public string Habitat { get; set; }
        [JsonPropertyName("ImageUrl")]
        public string ImageUrl { get; set; }
        [JsonPropertyName("IsExtinct")]
        public bool IsExtinct { get; set; }
        [JsonPropertyName("DiscoveredAt")]
        public DateTime? DiscoveredAt { get; set; }

        [JsonPropertyName("AverageLifeSpan")]
        public float AverageLifeSpan { get; set; }
    }
}
