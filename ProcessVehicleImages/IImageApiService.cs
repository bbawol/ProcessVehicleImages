using System.Collections.Generic;

namespace ProcessVehicleImages
{
    public interface IImageApiService
    {
        string ApiUri { get; set; }
        string ApiKey { get; set; }
        byte[] ImageData { set; }
        Dictionary<string, string> ExtractedFields { get; set; }
        Dictionary<string, string> InvokeApiService();
    }
}
