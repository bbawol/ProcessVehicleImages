using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace ProcessVehicleImages
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter image file path: ");
            string imageFilePath = Console.ReadLine();
            if (string.IsNullOrEmpty(imageFilePath))
                imageFilePath = @"C:\Users\brian\Downloads\Images\fedex.jpg";

            // C:\Users\brian\Downloads\Images\fedex.jpg     // FedEx truck
            // C:\Users\brian\Downloads\Images\00w0w_nSOqjjf0l5_600x450.jpg // Craigslist Ad
            // C:\Users\brian\Downloads\Images\acura.jpg             // acura car 
            MakeOCRRequest(imageFilePath);

            Console.WriteLine("\n\n\nHit ENTER to exit...");
            Console.ReadLine();
        }

        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }


        static async void MakeOCRRequest(string imageFilePath)
        {
            var client = new HttpClient();

            // Request headers. Replace the example key with a valid subscription key.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "3520fc34cd1142deb13b9f961746b9c1");

            // Request parameters and URI
            const string requestParameters = "language=en&detectOrientation=true";

            // NOTE: You must use the same location in your REST call as you used to obtain your subscription keys.
            //   For example, if you obtained your subscription keys from westus, replace "westcentralus" in the 
            //   URI below with "westus".
            const string uri = "https://westus.api.cognitive.microsoft.com/vision/v1.0/ocr?" + requestParameters;

            // Request body. Try this sample with a locally stored JPEG image.
            var byteData = GetImageAsByteArray(imageFilePath);

            using (var content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var response = await client.PostAsync(uri, content);
                var outputString = await response.Content.ReadAsStringAsync();
                var theOutput = JsonConvert.DeserializeObject(outputString);
                Console.WriteLine(theOutput);
            }
        }

    }
}
