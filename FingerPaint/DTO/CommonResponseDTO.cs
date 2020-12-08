using System;
using Newtonsoft.Json;

namespace FingerPaint.DTO
{
    public class CommonResponseDTO
    {
        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        public Action Positive { get; set; }

        public CommonResponseDTO()
        {
        }
    }
}
