using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ProcessVehicleImages
{
    public class MicrosoftOcrImageApi : IImageApiService
    {
        #region Properties

        public Dictionary<string, string> ExtractedFields { get; set; }
        public string ApiUri { get; set; }
        public string ApiKey { get; set; }
        public string ApiParams { get; set; }
        public string ApiHeader { get; set; }
        public byte[] ImageData { private get; set; } 

        #endregion

        public Dictionary<string, string> InvokeApiService()
        {
            if (ExtractedFields == null)
                ExtractedFields = new Dictionary<string, string>();

            var t = Task.Run(GenerateOutput);
            t.Wait();
            return t.Result;
        }

        private async Task<Dictionary<string, string>> GenerateOutput()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(ApiHeader, ApiKey);

            // NOTE: You must use the same location in your REST call as you used to obtain your subscription keys.
            //   For example, if you obtained your subscription keys from westus, replace "westcentralus" in the 
            //   URI below with "westus".
            var uri = string.Concat(ApiUri,ApiParams);
            var wordCount = 0;

            using (var content = new ByteArrayContent(ImageData))
            {

                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                var response = await client.PostAsync(uri, content);
                var outputString = await response.Content.ReadAsStringAsync();
                var theOutput = JsonConvert.DeserializeObject(outputString);
                var imageData = JObject.Parse(theOutput.ToString());
                var regions = (from d in imageData["regions"] select d).ToList();

                foreach (var region in regions)
                {
                    foreach (var line in region["lines"])
                    {
                        foreach (var word in line["words"])
                        {
                            var parsedWord = word["text"];
                            ExtractedFields.Add("FoundWord-"+wordCount++, parsedWord.ToString());
                        }
                    }
                }
                Debug.WriteLine(theOutput);
            }
            return ExtractedFields;
        }
    }
}
