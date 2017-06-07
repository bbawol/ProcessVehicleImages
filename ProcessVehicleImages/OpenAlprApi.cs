using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenAlprApi.Api;
using OpenAlprApi.Model;

namespace ProcessVehicleImages
{
    public class OpenAlprApi : IImageApiService
    {
        public string ApiUri { get; set; }
        public string ApiKey { get; set; }

        public byte[] ImageData { private get; set; }

        public Dictionary<string, string> ExtractedFields { get; set; }

        public Dictionary<string, string> InvokeApiService()
        {
            if (ExtractedFields == null)
                ExtractedFields = new Dictionary<string, string>();

            var apiInstance = new DefaultApi();
            var imageBytes = Convert.ToBase64String(ImageData);  // string | The image file that you wish to analyze encoded in base64 
            var secretKey = ApiKey;  // string | The secret key used to authenticate your account.  You can view your  secret key by visiting  https://cloud.openalpr.com/ 
            var country = "us";  // string | Defines the training data used by OpenALPR.  \"us\" analyzes  North-American style plates.  \"eu\" analyzes European-style plates.  This field is required if using the \"plate\" task  You may use multiple datasets by using commas between the country  codes.  For example, 'au,auwide' would analyze using both the  Australian plate styles.  A full list of supported country codes  can be found here https://github.com/openalpr/openalpr/tree/master/runtime_data/config 
            var recognizeVehicle = 56;  // int? | If set to 1, the vehicle will also be recognized in the image This requires an additional credit per request  (optional)  (default to 0)
            var state = "";  // string | Corresponds to a US state or EU country code used by OpenALPR pattern  recognition.  For example, using \"md\" matches US plates against the  Maryland plate patterns.  Using \"fr\" matches European plates against  the French plate patterns.  (optional)  (default to )
            var returnImage = 56;  // int? | If set to 1, the image you uploaded will be encoded in base64 and  sent back along with the response  (optional)  (default to 0)
            var topn = 56;  // int? | The number of results you would like to be returned for plate  candidates and vehicle classifications  (optional)  (default to 10)
            var prewarp = "";  // string | Prewarp configuration is used to calibrate the analyses for the  angle of a particular camera.  More information is available here http://doc.openalpr.com/accuracy_improvements.html#calibration  (optional)  (default to )

            try
            {
                InlineResponse200 result = apiInstance.RecognizeBytes(imageBytes, secretKey.ToString(), country, recognizeVehicle, state, returnImage, topn, prewarp);
                Debug.WriteLine(result);

                foreach (var plate in result.Results)
                {
                    ExtractedFields.Add("Region", plate.Region);
                    ExtractedFields.Add("Plate", plate.Plate);
                }
                return ExtractedFields;
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DefaultApi.RecognizeBytes: " + e.Message);
            }
            return null;
        }
    }
}
