using System;
using System.Collections.Generic;

namespace ProcessVehicleImages
{
    public class ImageData
    {
        public Guid ImageId { get; set; }
        public byte[] ImageBytes { get; set; }
        public List<ImageDataDetail> ImageDataValues { get; set; }
    }

    public class ImageDataDetail
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
