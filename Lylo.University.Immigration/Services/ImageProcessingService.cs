using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Lylo.University.Immigration.Services
{
    public class ImageProcessingService : IDisposable
    {
        private readonly IComputerVisionClient _computerVisionClient;

        public ImageProcessingService(IComputerVisionClient computerVisionClient)
        {
            _computerVisionClient = computerVisionClient ?? throw new ArgumentNullException(nameof(computerVisionClient));
        }

        public async Task<string> ExtractTextFromImageAsync(Stream imageStream)
        {
            try
            {
                var features = new List<VisualFeatureTypes?>
                {
                    VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
                    VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
                    VisualFeatureTypes.Tags, VisualFeatureTypes.Adult,
                    VisualFeatureTypes.Color, VisualFeatureTypes.Brands,
                    VisualFeatureTypes.Objects
                };

              
                using (imageStream)
                {
                   
                    var result = await _computerVisionClient.AnalyzeImageInStreamAsync(imageStream, features, null, null, null, null, CancellationToken.None);
                    
                    var description = result.Description.Captions.FirstOrDefault()?.Text;
                    var tags = result.Tags.Select(tag => tag.Name);
                    // Отримання розпізнаних категорій
                    var categories = result.Categories.Select(c => c.Name);

                    // Отримання розпізнаних облич
                    var faces = result.Faces.Select(f => $"Face rectangle: {f.FaceRectangle.Width}x{f.FaceRectangle.Height}, Age: {f.Age}, Gender: {f.Gender}");

                    // Отримання типу зображення
                    var imageType = result.ImageType.ClipArtType == 0 ? "Not ClipArt" : "ClipArt";
                    imageType += ", " + (result.ImageType.LineDrawingType == 0 ? "Not LineDrawing" : "LineDrawing");

                    // Отримання оцінки для дорослих
                    var adultContent = $"Is Adult Content: {result.Adult.IsAdultContent}, Adult Score: {result.Adult.AdultScore}, " +
                                       $"Is Racy Content: {result.Adult.IsRacyContent}, Racy Score: {result.Adult.RacyScore}";

                    // Отримання інформації про кольори
                    var colors = result.Color.DominantColors;

                    // Отримання розпізнаних брендів
                    var brands = result.Brands.Select(b => b.Name);

                    // Отримання розпізнаних об'єктів
                    var objects = result.Objects.Select(o => $"Object: {o.ObjectProperty}, Rectangle: {o.Rectangle.X},{o.Rectangle.Y},{o.Rectangle.W},{o.Rectangle.H}");

                    // Формування результуючого тексту з усіма розпізнаними особливостями
                    var extractedText = $"Description: {description}\n" +
                                        $"Tags: {string.Join(", ", tags)}\n" +
                                        $"Categories: {string.Join(", ", categories)}\n" +
                                        $"Faces: {string.Join("; ", faces)}\n" +
                                        $"Image Type: {imageType}\n" +
                                        $"Adult Content: {adultContent}\n" +
                                        $"Colors: {string.Join(", ", colors)}\n" +
                                        $"Brands: {string.Join(", ", brands)}\n" +
                                        $"Objects: {string.Join("; ", objects)}";

                    return extractedText;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during image processing: {ex.Message}");
                Console.WriteLine(ex.StackTrace);

                return string.Empty;
            }
        }

        public void Dispose()
        {
            _computerVisionClient.Dispose();
        }
    }
}


