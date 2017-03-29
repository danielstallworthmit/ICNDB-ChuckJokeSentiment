using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network
{

    public class SentimentRequest
    {
        [JsonProperty(PropertyName = "documents")]
        public Document[] Documents { get; set; }
    }

    public class Document
    {
        [JsonProperty(PropertyName = "language")]
        public string Language { get; set; }
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
    }

}
