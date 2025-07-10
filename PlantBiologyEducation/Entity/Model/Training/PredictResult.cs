using System.Text.Json.Serialization;

namespace PlantBiologyEducation.Entity.Model.Training
{
    public class Box
    {
        [JsonPropertyName("x1")]
        public double X1 { get; set; }
        [JsonPropertyName("y1")]
        public double Y1 { get; set; }
        [JsonPropertyName("x2")]
        public double X2 { get; set; }
        [JsonPropertyName("y2")]
        public double Y2 { get; set; }
    }

    public class PredictResult
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("class")]
        public int Class { get; set; }
        [JsonPropertyName("confidence")]
        public double Confidence { get; set; }
        [JsonPropertyName("box")]
        public Box Box { get; set; }
    }

    // Lớp để kết hợp kết quả dự đoán và chi tiết loài
    public class PredictionWithDetail
    {
        public PredictResult Prediction { get; set; }
        public SpeciesDetail Detail { get; set; }
    }

}
