﻿using System;
using Newtonsoft.Json;

namespace FingerPaint.Reponse
{
    public class ReponseGet
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("fileId")]
        public string FileId { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("linkFile")]
        public string LinkFile { get; set; }
    }
}
