using System.Collections.Generic;

namespace ProcessVehicleImages
{
    public class ImageProcessor
    {
        private readonly IImageApiService _apiService;

        public ImageProcessor(IImageApiService apiService)
        {
            _apiService = apiService;
        }
        
        public Dictionary<string, string> ExtractImageDetails()
        {
            return _apiService.InvokeApiService();
        }
    }
}
