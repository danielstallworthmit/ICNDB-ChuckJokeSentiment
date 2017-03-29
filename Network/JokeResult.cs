using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network
{

    public class JokeResult
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "value")]
        public Joke[] Jokes { get; set; }
        public double AverageSentimentScore { get; set; }
        public string Category { get; set; }
    }
}
