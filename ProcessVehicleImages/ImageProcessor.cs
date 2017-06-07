using System;
using System.Collections.Generic;

namespace ProcessVehicleImages
{
    public class ImageProcessor
    {
        private static readonly ImageData ImageData = new ImageData();
        private readonly IImageApiService _apiService;

        public ImageProcessor(IImageApiService apiService)
        {
            _apiService = apiService;

        }
        
        //private static void ImageDataOnInfoParsed(object sender, InfoParsedEventArgs infoParsedEventArgs)
        //{
        //    ProcessLicensePlate(infoParsedEventArgs.ImageDataBytes);
        //}

        //async void MakeOcrRequest(byte[] byteData)
        //{
        //    //var client = new HttpClient();

        //    //client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", msftKey);

        //    //// NOTE: You must use the same location in your REST call as you used to obtain your subscription keys.
        //    ////   For example, if you obtained your subscription keys from westus, replace "westcentralus" in the 
        //    ////   URI below with "westus".
        //    //var uri = "https://westus.api.cognitive.microsoft.com/vision/v1.0/ocr?" + ocrParams;

        //    //var args = new InfoParsedEventArgs
        //    //{
        //    //    ImageDataBytes = byteData,
        //    //    InfoData = new List<ImageDataDetail>()
        //    //};

        //    //using (var content = new ByteArrayContent(byteData))
        //    //{
        //    //    // This example uses content type "application/octet-stream".
        //    //    // The other content types you can use are "application/json" and "multipart/form-data".
        //    //    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        //    //    var response = await client.PostAsync(uri, content);
        //    //    var outputString = await response.Content.ReadAsStringAsync();
        //    //    var theOutput = JsonConvert.DeserializeObject(outputString);
        //    //    var m = JsonConvert.DeserializeObject(theOutput.ToString());

        //    //    args.InfoData.Add(new ImageDataDetail { Key = "", Value = "" });


        //    //    Console.WriteLine(theOutput);
        //    //    Debug.WriteLine(theOutput);
        //    //}

        //    //OnInfoParsed(args);

        //}

        public event EventHandler<InfoParsedEventArgs> InfoParsed;
        
        //private static void ProcessLicensePlate(byte[] imageByteArray)
        //{
        //    var apiInstance = new DefaultApi();
        //    var imageBytes = Convert.ToBase64String(imageByteArray);  // string | The image file that you wish to analyze encoded in base64 
        //    //var secretKey = _appSettingsReader.GetValue("OpenAlprKey", typeof(string));  // string | The secret key used to authenticate your account.  You can view your  secret key by visiting  https://cloud.openalpr.com/ 
        //    var country = "us";  // string | Defines the training data used by OpenALPR.  \"us\" analyzes  North-American style plates.  \"eu\" analyzes European-style plates.  This field is required if using the \"plate\" task  You may use multiple datasets by using commas between the country  codes.  For example, 'au,auwide' would analyze using both the  Australian plate styles.  A full list of supported country codes  can be found here https://github.com/openalpr/openalpr/tree/master/runtime_data/config 
        //    var recognizeVehicle = 56;  // int? | If set to 1, the vehicle will also be recognized in the image This requires an additional credit per request  (optional)  (default to 0)
        //    var state = "";  // string | Corresponds to a US state or EU country code used by OpenALPR pattern  recognition.  For example, using \"md\" matches US plates against the  Maryland plate patterns.  Using \"fr\" matches European plates against  the French plate patterns.  (optional)  (default to )
        //    var returnImage = 56;  // int? | If set to 1, the image you uploaded will be encoded in base64 and  sent back along with the response  (optional)  (default to 0)
        //    var topn = 56;  // int? | The number of results you would like to be returned for plate  candidates and vehicle classifications  (optional)  (default to 10)
        //    var prewarp = "";  // string | Prewarp configuration is used to calibrate the analyses for the  angle of a particular camera.  More information is available here http://doc.openalpr.com/accuracy_improvements.html#calibration  (optional)  (default to )

        //    try
        //    {
        //        InlineResponse200 result = apiInstance.RecognizeBytes(imageBytes, secretKey.ToString(), country, recognizeVehicle, state, returnImage, topn, prewarp);
        //        Console.WriteLine(result);
        //        Debug.WriteLine(result);

        //        //var outputString = "Results count = " + result.Results;

        //        foreach (var plate in result.Results)
        //        {
        //            // outputString += Environment.NewLine + "Region = " + plate.Region + " Plate = " + plate.Plate;
        //            ImageData.ImageDataValues.Add(new ImageDataDetail { Key = "Region", Value = plate.Region });
        //            ImageData.ImageDataValues.Add(new ImageDataDetail { Key = "Plate", Value = plate.Plate });
        //        }
        //        //Console.WriteLine(outputString);
        //        //Debug.WriteLine(outputString);
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.Print("Exception when calling DefaultApi.RecognizeBytes: " + e.Message);
        //    }
        //}

        protected virtual void OnInfoParsed(InfoParsedEventArgs e)
        {
            EventHandler<InfoParsedEventArgs> handler = InfoParsed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public Dictionary<string, string> ExtractImageDetails()
        {
            return _apiService.InvokeApiService();
        }
    }

    public class InfoParsedEventArgs : EventArgs
    {
        public List<ImageDataDetail> InfoData { get; set; }
        public byte[] ImageDataBytes { get; set; }
    }
}
