﻿using Newtonsoft.Json;

namespace UCS.Extensions.Http.Sender.Entities
{
    public class HttpSenderOptions
    {
        public bool ValidateErrorsInResponse { get; set; }
        public JsonSerializerSettings SerializingSettings { get; set; } = new JsonSerializerSettings();
        public JsonSerializerSettings DeserializingSettings { get; set; } = new JsonSerializerSettings();
    }
}
