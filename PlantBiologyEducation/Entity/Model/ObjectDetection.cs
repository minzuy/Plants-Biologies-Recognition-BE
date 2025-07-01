using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing;    
using SixLabors.ImageSharp.Processing;
using System.Text.Json;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing.Processing;

namespace PlantBiologyEducation.ObjectDetections
{
    public class PredictionResult
    {
        public string Label { get; set; }
        public float Confidence { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
    }

    public class ObjectDetection
    {
        private readonly InferenceSession _session;
        private readonly List<string> _labels;

        public ObjectDetection(string modelPath, List<string> labels)
        {
            _session = new InferenceSession(modelPath);
            _labels = labels;
        }

        public (byte[] resultImage, List<PredictionResult> predictions) Predict(byte[] imageBytes, float confidenceThreshold = 0.5f)
        {
            using var image = Image.Load<Rgb24>(imageBytes);
            var resized = image.Clone(ctx => ctx.Resize(new ResizeOptions
            {
                Size = new Size(640, 640),
                Mode = ResizeMode.Stretch
            }));

            var inputTensor = ExtractPixels(resized);
            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("images", inputTensor)
            };

            using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = _session.Run(inputs);
            var output = results.First().AsEnumerable<float>().ToArray();

            var predictions = ParseOutput(output, confidenceThreshold);
            var resultImage = DrawBoxes(resized, predictions);

            using var ms = new MemoryStream();
            resultImage.SaveAsJpeg(ms);
            return (ms.ToArray(), predictions);
        }

        private DenseTensor<float> ExtractPixels(Image<Rgb24> image)
        {
            var tensor = new DenseTensor<float>(new[] { 1, 3, 640, 640 });
            for (int y = 0; y < 640; y++)
            {
                for (int x = 0; x < 640; x++)
                {
                    var pixel = image[x, y];
                    tensor[0, 0, y, x] = pixel.R / 255f;
                    tensor[0, 1, y, x] = pixel.G / 255f;
                    tensor[0, 2, y, x] = pixel.B / 255f;
                }
            }
            return tensor;
        }

        private List<PredictionResult> ParseOutput(float[] output, float threshold)
        {
            var results = new List<PredictionResult>();
            int dimensions = 85;
            int rows = output.Length / dimensions;

            for (int i = 0; i < rows; i++)
            {
                float conf = output[i * dimensions + 4];
                if (conf < threshold) continue;

                var classScores = output.Skip(i * dimensions + 5).Take(27).ToArray();
                int classId = Array.IndexOf(classScores, classScores.Max());
                float classConfidence = classScores[classId];

                if (classConfidence < threshold) continue;

                results.Add(new PredictionResult
                {
                    Label = _labels[classId],
                    Confidence = classConfidence,
                    X = output[i * dimensions],
                    Y = output[i * dimensions + 1],
                    Width = output[i * dimensions + 2],
                    Height = output[i * dimensions + 3]
                });
            }
            return results;
        }

        private Image<Rgb24> DrawBoxes(Image<Rgb24> image, List<PredictionResult> results)
        {
            var font = SystemFonts.CreateFont("Arial", 16); // SystemFonts is part of SixLabors.Fonts
            image.Mutate(ctx =>
            {
                foreach (var result in results)
                {
                    var rect = new RectangleF(
                        result.X - result.Width / 2,
                        result.Y - result.Height / 2,
                        result.Width,
                        result.Height);

                    ctx.Draw(Pens.Solid(Color.Red, 2), rect);
                    ctx.DrawText($"{result.Label} ({result.Confidence:P1})", font, Color.Yellow, new PointF(rect.X, rect.Y - 20)); 
                }
            });
            return image;
        }
    }
}
