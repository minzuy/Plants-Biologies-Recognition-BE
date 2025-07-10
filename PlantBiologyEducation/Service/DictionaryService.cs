using PlantBiologyEducation.Entity.Model.Training;
using System.Text.Json;
using System.IO;

namespace PlantBiologyEducation.Service
{
    public class DictionaryService
    {
        public Dictionary<string, SpeciesDetail> LoadSpeciesData()
        {
            var json = File.ReadAllText("species-data.json");
            return JsonSerializer.Deserialize<Dictionary<string, SpeciesDetail>>(json);
        }
    }
}
