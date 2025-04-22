using System;
using System.Net;

namespace AssetStreaming
{ 
    [Serializable]
    public class HttpResponse
    {
        public WebHeaderCollection headers;
        public string responseBody;
    }
}