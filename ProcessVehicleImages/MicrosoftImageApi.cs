using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProcessVehicleImages
{
    public class MicrosoftImageApi : IImageApiService
    {
        public Dictionary<string, string> ExtractedFields { get; set; }

        public string ApiUri { get; set; }
        public string ApiKey { get; set; }
        public string ApiParams { get; set; }

        public byte[] ImageData { private get; set; }

        public Dictionary<string, string> InvokeApiService()
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ApiKey);

            // NOTE: You must use the same location in your REST call as you used to obtain your subscription keys.
            //   For example, if you obtained your subscription keys from westus, replace "westcentralus" in the 
            //   URI below with "westus".
            var uri = ApiUri + ApiParams;

            using (var content = new ByteArrayContent(ImageData))
            {
                var t = Task.Run(async () =>
                {
                    // This example uses content type "application/octet-stream".
                    // The other content types you can use are "application/json" and "multipart/form-data".
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    var response = await client.PostAsync(uri, content);
                    var outputString = await response.Content.ReadAsStringAsync();

                    var theOutput = JsonConvert.DeserializeObject(outputString);
                    var m = JsonConvert.DeserializeObject(theOutput.ToString());

                    ExtractedFields.Add("aaa", "blah");

                    Console.WriteLine(theOutput);
                    Debug.WriteLine(theOutput);

                }); // end task

                t.Wait();

                return ExtractedFields;

            }
        }
    }
}
