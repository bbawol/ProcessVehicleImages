using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using OpenAlprApi.Api;
using OpenAlprApi.Model;

namespace ProcessVehicleImages
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter image file path: ");

            string imageFilePath = Console.ReadLine();
            if (string.IsNullOrEmpty(imageFilePath))
                imageFilePath = @"https://images.craigslist.org/00w0w_nSOqjjf0l5_1200x900.jpg"; //  @"C:\Users\brian\Downloads\Images\mustang.jpg";

            imageFilePath = @"C:\Users\brian\Downloads\Images\audi.jpg";
            
            Console.WriteLine("Image : " + imageFilePath);
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
            // var byteData = GetImageAsByteArray(imageFilePath);
            var byteDate = DownloadRemoteImageFile(imageFilePath);

            using (var content = new ByteArrayContent(byteDate))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var response = await client.PostAsync(uri, content);
                var outputString = await response.Content.ReadAsStringAsync();
                var theOutput = JsonConvert.DeserializeObject(outputString);
                Console.WriteLine(theOutput);
                Debug.WriteLine(theOutput);
            }

            ProcessLicensePlate(byteDate);
        }

        private static byte[] DownloadRemoteImageFile(string uri)
        {
            var webClient = new WebClient();
            byte[] imageBytes = webClient.DownloadData(uri);
            return imageBytes;
        }

        private static void ProcessLicensePlate(byte[] imageByteArray)
        {
            var apiInstance = new DefaultApi();
            var imageBytes = Convert.ToBase64String(imageByteArray);  // string | The image file that you wish to analyze encoded in base64 
            var secretKey = "";  // string | The secret key used to authenticate your account.  You can view your  secret key by visiting  https://cloud.openalpr.com/ 
            var country = "us";  // string | Defines the training data used by OpenALPR.  \"us\" analyzes  North-American style plates.  \"eu\" analyzes European-style plates.  This field is required if using the \"plate\" task  You may use multiple datasets by using commas between the country  codes.  For example, 'au,auwide' would analyze using both the  Australian plate styles.  A full list of supported country codes  can be found here https://github.com/openalpr/openalpr/tree/master/runtime_data/config 
            var recognizeVehicle = 56;  // int? | If set to 1, the vehicle will also be recognized in the image This requires an additional credit per request  (optional)  (default to 0)
            var state = "";  // string | Corresponds to a US state or EU country code used by OpenALPR pattern  recognition.  For example, using \"md\" matches US plates against the  Maryland plate patterns.  Using \"fr\" matches European plates against  the French plate patterns.  (optional)  (default to )
            var returnImage = 56;  // int? | If set to 1, the image you uploaded will be encoded in base64 and  sent back along with the response  (optional)  (default to 0)
            var topn = 56;  // int? | The number of results you would like to be returned for plate  candidates and vehicle classifications  (optional)  (default to 10)
            var prewarp = "";  // string | Prewarp configuration is used to calibrate the analyses for the  angle of a particular camera.  More information is available here http://doc.openalpr.com/accuracy_improvements.html#calibration  (optional)  (default to )

            try
            {
                InlineResponse200 result = apiInstance.RecognizeBytes(imageBytes, secretKey, country, recognizeVehicle, state, returnImage, topn, prewarp);
                Console.WriteLine(result);
                Debug.WriteLine(result);

                var outputString = "Results count = " + result.Results;

                foreach (var plate in result.Results)
                {
                    outputString += Environment.NewLine + "Region = " + plate.Region + " Plate = " + plate.Plate;
                }
                Console.WriteLine(outputString);
                Debug.WriteLine(outputString);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DefaultApi.RecognizeBytes: " + e.Message);
            }
        }

    }
}
