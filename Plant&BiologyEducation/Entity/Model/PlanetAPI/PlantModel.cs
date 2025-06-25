using System.ComponentModel.DataAnnotations;

namespace Plant_BiologyEducation.Entity.Model.MyPlant
{
    public class Species
    {
        public string ScientificName { get; set; }
        public List<string> CommonNames { get; set; }
    }

    public class Result
    {
        public double Score { get; set; }
        public Species Species { get; set; }
    }

    public class PlantNetResponse
    {
        public string BestMatch { get; set; }
        public List<Result> Results { get; set; }
    }
}