using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network
{

    public class SentimentResponse
    {
        [JsonProperty(PropertyName = "documents")]
        public Sentiment[] Sentiments { get; set; }
    }

    public class Sentiment
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "score")]
        public string Score { get; set; }
    }

}
