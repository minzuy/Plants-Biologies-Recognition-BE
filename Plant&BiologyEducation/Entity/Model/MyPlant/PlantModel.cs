using System.ComponentModel.DataAnnotations;

namespace Plant_BiologyEducation.Entity.Model.MyPlant
{
    // Request Model
    public class PlantIdentificationRequest
    {
        /// <summary>
        /// File ảnh thực vật cần nhận dạng
        /// </summary>
        [Required(ErrorMessage = "Vui lòng chọn file ảnh")]
        public IFormFile Image { get; set; }

        /// <summary>
        /// Project/region để nhận dạng (mặc định: all)
        /// </summary>
        public string Project { get; set; } = "all";

        /// <summary>
        /// Loại bộ phận thực vật (leaf, flower, fruit, bark)
        /// </summary>
        [RegularExpression("^(leaf|flower|fruit|bark)$", ErrorMessage = "Organ phải là: leaf, flower, fruit, hoặc bark")]
        public string Organ { get; set; } = "leaf";
    }

    // PlantNet API Response Models
    public class PlantApiResponse
    {
        public string Query { get; set; }
        public string Language { get; set; }
        public int PreferedReferential { get; set; }
        public bool BestMatch { get; set; }
        public List<PlantNetResult> Results { get; set; }
        public int RemainingIdentificationRequests { get; set; }
    }

    public class PlantNetResult
    {
        public double Score { get; set; }
        public PlantNetSpecies Species { get; set; }
        public List<PlantNetImage> Images { get; set; }
        public string Gbif { get; set; }
    }

    public class PlantNetSpecies
    {
        public string ScientificNameWithoutAuthor { get; set; }
        public string ScientificNameAuthorship { get; set; }
        public PlantNetGenus Genus { get; set; }
        public PlantNetFamily Family { get; set; }
        public List<PlantNetCommonName> CommonNames { get; set; }
    }

    public class PlantNetGenus
    {
        public string ScientificNameWithoutAuthor { get; set; }
        public string ScientificNameAuthorship { get; set; }
    }

    public class PlantNetFamily
    {
        public string ScientificNameWithoutAuthor { get; set; }
        public string ScientificNameAuthorship { get; set; }
    }

    public class PlantNetCommonName
    {
        public string Lang { get; set; }
        public string Name { get; set; }
    }

    public class PlantNetImage
    {
        public string Organ { get; set; }
        public string Author { get; set; }
        public string License { get; set; }
        public PlantNetUrl Url { get; set; }
    }

    public class PlantNetUrl
    {
        public string O { get; set; } // Original
        public string M { get; set; } // Medium  
        public string S { get; set; } // Small
    }

    // Response Models
    public class PlantIdentificationResult
    {
        public bool IsSuccess { get; set; }
        public string PlantName { get; set; }
        public string ScientificName { get; set; }
        public double Probability { get; set; }
        public List<PlantSuggestion> Suggestions { get; set; }
        public List<SimilarImage> SimilarImages { get; set; }
        public string ErrorMessage { get; set; }
        public int RemainingRequests { get; set; }
    }

    public class PlantSuggestion
    {
        public string PlantName { get; set; }
        public string ScientificName { get; set; }
        public double Probability { get; set; }
        public List<string> CommonNames { get; set; }
    }

    public class SimilarImage
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public double Similarity { get; set; }
    }
}