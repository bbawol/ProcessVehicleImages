using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;

namespace ProcessVehicleImages
{
    class Program
    {
        public static AppSettingsReader AppSettingsReader;

        static void Main(string[] args)
        {
            AppSettingsReader = new AppSettingsReader();

            Console.Write("Enter image file path: ");

            var imageFilePath = Console.ReadLine();

            if (string.IsNullOrEmpty(imageFilePath))
                imageFilePath = AppSettingsReader.GetValue("DebugImageFilePath", typeof(string)).ToString();

            Console.WriteLine("Image : " + imageFilePath);

            var imageBytes = imageFilePath.ToString(CultureInfo.InvariantCulture).Contains(@"://") 
                ? ImageDownloader.DownloadRemoteImageFile(imageFilePath) 
                : ImageDownloader.GetImageFileAsByteArray(imageFilePath);

            if (imageBytes == null || imageBytes.Length == 0)
            {
                Console.WriteLine("The image data is not valid");
                return;
            }

            #region Microsoft Service Call

            var msftUri = AppSettingsReader.GetValue("MsftUri", typeof(string)).ToString();
            var msftKey = AppSettingsReader.GetValue("MsftApiSubscriptionKey", typeof(string)).ToString();
            var msftParams = AppSettingsReader.GetValue("MsftRequestParams", typeof(string)).ToString();
            var msftHeader = AppSettingsReader.GetValue("MssftApiHeader", typeof(string)).ToString();

            var microsoftProcessor = new ImageProcessor(new MicrosoftImageApi
            {
                ApiUri = msftUri,
                ApiKey = msftKey,
                ApiParams = msftParams,
                ImageData = imageBytes,
                ApiHeader = msftHeader
            });

            var msftList = microsoftProcessor.ExtractImageDetails();

            #endregion

            #region OpenAlpr Service Call

            var openAlprUri = AppSettingsReader.GetValue("OpenAlprUri", typeof(string)).ToString();
            var openAlprKey = AppSettingsReader.GetValue("OpenAlprKey", typeof(string)).ToString();

            var openAlprProcessor = new ImageProcessor(new OpenAlprApi
            {
                ApiUri = openAlprUri,
                ApiKey = openAlprKey,
                ImageData = imageBytes
            });

            var alprList = openAlprProcessor.ExtractImageDetails();

            #endregion

            var finalList = msftList.Concat(alprList).GroupBy(d => d.Key)
             .ToDictionary(d => d.Key, d => d.First().Value);

            foreach (var keyValuePair in finalList)
                Console.WriteLine("\n\nKey=" + keyValuePair.Key + " | Value=" + keyValuePair.Value);

            Console.WriteLine("\n\n\nHit ENTER to exit...");
            Console.ReadLine();
        }
    }

    public class ImageDownloader
    {
        public static byte[] GetImageFileAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

        public static byte[] DownloadRemoteImageFile(string uri)
        {
            var webClient = new WebClient();
            var imageBytes = webClient.DownloadData(uri);
            return imageBytes;
        }
    }
}
