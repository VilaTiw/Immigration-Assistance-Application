//using Lylo.University.Immigration.Services;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.IO;
//using System.Threading.Tasks;

//namespace Lylo.University.Immigration.Controllers
//{
//    [ApiController]
//    [Route("api/images")]
//    public class ImageProcessingController : Controller
//    {
//        private readonly ImageProcessingService imageProcessingService;

//        public ImageProcessingController(ImageProcessingService imageProcessingService)
//        {
//            this.imageProcessingService = imageProcessingService;
//        }

//        [HttpPost]
//        public async Task<IActionResult> ProcessImage(IFormFile file)
//        {
//            try
//            {
//                if (file == null || file.Length == 0)
//                    return BadRequest("No file uploaded.");

//                var imageUrl = Path.GetTempFileName();
//                using (var stream = new FileStream(imageUrl, FileMode.Create))
//                {
//                    await file.CopyToAsync(stream);
//                }

//                var extractedText = await imageProcessingService.ExtractTextFromImageAsync(imageUrl);

//                // Повернення результату користувачу
//                return Ok(extractedText);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
//            }
//        }
//    }
//}
