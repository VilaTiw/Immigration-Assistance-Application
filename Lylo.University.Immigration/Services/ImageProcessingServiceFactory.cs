using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using System;

namespace Lylo.University.Immigration.Services
{
    public class ImageProcessingServiceFactory
    {
        private readonly IComputerVisionClient _computerVisionClient;

        public ImageProcessingServiceFactory(IComputerVisionClient computerVisionClient)
        {
            _computerVisionClient = computerVisionClient ?? throw new ArgumentNullException(nameof(computerVisionClient));
        }

        public ImageProcessingService CreateImageProcessingService()
        {
            return new ImageProcessingService(_computerVisionClient);
        }
    }
}