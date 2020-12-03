using System;
using Newtonsoft.Json;

namespace FingerPaint.Request
{
    public class RequestGet
    {
        [JsonProperty("fileId")]
        public string FileId { get; set; }
    }
}
