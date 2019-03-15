using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace UCS.Extensions.Http.Client
{

    public class HttpClientOptionsProvider
    {
        public Uri BaseAddress { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public TimeSpan Timeout { get; set; }

        public List<MediaTypeWithQualityHeaderValue> DefaultRequestHeaders = new List<MediaTypeWithQualityHeaderValue>();

        public void AddJsonDefaultRequestHeader()
        {
            DefaultRequestHeaders.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void AddXmlDefaultRequestHeader()
        {
            DefaultRequestHeaders.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
        }

    }
}
