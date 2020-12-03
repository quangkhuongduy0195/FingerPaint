using System;
using Newtonsoft.Json;

namespace FingerPaint.Reponse
{
    public class ReponseSave
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

    }
}
