using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Processing;
using System.Text.Json;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing.Processing;

namespace Plant_BiologyEducation.Entity.Model.Training
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
            int originalWidth = image.Width;
            int originalHeight = image.Height;

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

            // Scale bounding boxes back to original image size
            float scaleX = (float)originalWidth / 640;
            float scaleY = (float)originalHeight / 640;

            foreach (var prediction in predictions)
            {
                prediction.X *= scaleX;
                prediction.Y *= scaleY;
                prediction.Width *= scaleX;
                prediction.Height *= scaleY;
            }

            var filteredPredictions = ApplyNMS(predictions, 0.4f);

            // Draw boxes on resized image for display (optional)
            var resultImage = DrawBoxes(resized, filteredPredictions.Select(p =>
                new PredictionResult
                {
                    Label = p.Label,
                    Confidence = p.Confidence,
                    X = p.X / scaleX,
                    Y = p.Y / scaleY,
                    Width = p.Width / scaleX,
                    Height = p.Height / scaleY
                }).ToList());

            using var ms = new MemoryStream();
            resultImage.SaveAsJpeg(ms);
            return (ms.ToArray(), filteredPredictions);
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
            int dimensions = 85; // YOLOv5 output format
            int rows = output.Length / dimensions;

            for (int i = 0; i < rows; i++)
            {
                int baseIndex = i * dimensions;

                float objectness = Sigmoid(output[baseIndex + 4]);
                if (objectness < threshold) continue;

                int numClasses = Math.Min(_labels.Count, dimensions - 5);
                var classScores = new float[numClasses];
                for (int j = 0; j < numClasses; j++)
                {
                    float classRaw = output[baseIndex + 5 + j];
                    classScores[j] = Sigmoid(classRaw) * objectness;
                }

                int classId = Array.IndexOf(classScores, classScores.Max());
                float classConfidence = classScores[classId];

                if (classConfidence < threshold || classId >= _labels.Count) continue;

                float centerX = output[baseIndex] * 640;
                float centerY = output[baseIndex + 1] * 640;
                float width = output[baseIndex + 2] * 640;
                float height = output[baseIndex + 3] * 640;

                results.Add(new PredictionResult
                {
                    Label = _labels[classId],
                    Confidence = classConfidence,
                    X = centerX,
                    Y = centerY,
                    Width = width,
                    Height = height
                });
            }

            return results.OrderByDescending(r => r.Confidence).ToList();
        }

        private List<PredictionResult> ApplyNMS(List<PredictionResult> predictions, float iouThreshold)
        {
            var result = new List<PredictionResult>();
            var suppressed = new bool[predictions.Count];

            for (int i = 0; i < predictions.Count; i++)
            {
                if (suppressed[i]) continue;
                result.Add(predictions[i]);

                for (int j = i + 1; j < predictions.Count; j++)
                {
                    if (suppressed[j]) continue;

                    float iou = CalculateIoU(predictions[i], predictions[j]);
                    if (iou > iouThreshold)
                    {
                        suppressed[j] = true;
                    }
                }
            }

            return result;
        }

        private float CalculateIoU(PredictionResult box1, PredictionResult box2)
        {
            float x1_1 = box1.X - box1.Width / 2;
            float y1_1 = box1.Y - box1.Height / 2;
            float x2_1 = box1.X + box1.Width / 2;
            float y2_1 = box1.Y + box1.Height / 2;

            float x1_2 = box2.X - box2.Width / 2;
            float y1_2 = box2.Y - box2.Height / 2;
            float x2_2 = box2.X + box2.Width / 2;
            float y2_2 = box2.Y + box2.Height / 2;

            float intersectionX1 = Math.Max(x1_1, x1_2);
            float intersectionY1 = Math.Max(y1_1, y1_2);
            float intersectionX2 = Math.Min(x2_1, x2_2);
            float intersectionY2 = Math.Min(y2_1, y2_2);

            float intersectionArea = Math.Max(0, intersectionX2 - intersectionX1) *
                                     Math.Max(0, intersectionY2 - intersectionY1);
            float area1 = box1.Width * box1.Height;
            float area2 = box2.Width * box2.Height;
            float unionArea = area1 + area2 - intersectionArea;

            return unionArea > 0 ? intersectionArea / unionArea : 0;
        }

        private Image<Rgb24> DrawBoxes(Image<Rgb24> image, List<PredictionResult> results)
        {
            if (results.Count == 0) return image;

            var font = SystemFonts.CreateFont("Arial", 16);
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

        private float Sigmoid(float x)
        {
            return 1 / (1 + (float)Math.Exp(-x));
        }
    }
}
