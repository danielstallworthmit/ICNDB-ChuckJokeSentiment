using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Network
{
    class Program
    {
        static void Main(string[] args)
        {
            // Want the average sentiment of various Chuck Norris categories (nerdy, etc)
            // Pull random jokes from the icndb for each category
            // Get x jokes for each category and get the sentiment for each joke and save it in a list
            var numberOfJokes = 10;
            var categories = "nerdy,explicit";
            var myJokes = GetXJokesForCategories(numberOfJokes, categories);
            // Compute average sentiment of each category list
            var categorySentimentAverage = new double[myJokes.Count];
            foreach (var jokeList in myJokes) {
                SentimentResponse sentimentResponse = GetSentimentResponse(jokeList.Jokes, numberOfJokes);
                foreach (var sentiment in sentimentResponse.Sentiments) {
                    foreach (var joke in jokeList.Jokes)
                    {
                        if (joke.Id == sentiment.Id)
                        {
                            double score;
                            if(double.TryParse(sentiment.Score, out score))
                            {
                                joke.SentimentScore = score;
                            }
                            break;
                            //System.Console.WriteLine(joke.Text);
                        }
                    }
                }
                //var averageSentimentScore = 0.0;
                //var sentimentSum = 0.0;
                //foreach (var joke in jokeList.Jokes) {
                //    sentimentSum += joke.SentimentScore;  // Get the total for the average calc
                //    //Console.WriteLine(string.Format("Sentiment Score: {0:p}, Joke ID: {1}, Joke: {2} \r\n", joke.SentimentScore, joke.Id, joke.Text));
                //    //Console.ReadKey();
                //}
                //averageSentimentScore = sentimentSum / jokeList.Jokes.Length;
                //jokeList.AverageSentimentScore = averageSentimentScore;
                jokeList.AverageSentimentScore = jokeList.Jokes.Average(j => j.SentimentScore);  // Using Linq average gets rid of 9 lines above
                Console.WriteLine(string.Format("Category: {0}, Average Sentiment: {1:p}", jokeList.Category, jokeList.AverageSentimentScore));
            }
            var orderedJokeList = OrderJokeListsByAverageSentiment(myJokes);
            foreach(var jokeList in orderedJokeList) {
                Console.WriteLine(string.Format("Ordered Category: {0}, Average Sentiment: {1:p}", jokeList.Category, jokeList.AverageSentimentScore));
            }

            // Sort by average sentiment descending

        }

        public static List<JokeResult> OrderJokeListsByAverageSentiment(List<JokeResult> jokeLists)
        {
            var orderedJokeLists = new List<JokeResult>();
            jokeLists.Sort(new JokeResultComparer());
            orderedJokeLists.AddRange(jokeLists);  // AddRange from Linq replaces 6 lines below

            //int counter = 0;
            //foreach(var jokeResult in jokeLists)
            //{
            //    orderedJokeLists.Add(jokeResult);
            //    counter++;
            //}

            return orderedJokeLists;
        }

        public static SentimentResponse GetSentimentResponse(Joke[] jokes, int numberOfJokes = 1)
        {
            // Set up sentiment request and response
            var sentimentResponse = new SentimentResponse();
            var sentimentRequest = new SentimentRequest();
            sentimentRequest.Documents = new Document[numberOfJokes];
            for(int i=0;i<jokes.Length;i++)
            {
                sentimentRequest.Documents[i] = new Document { Id = jokes[i].Id, Text = jokes[i].Text };
            }
            // Initialize webclient and add headers
            var webClient = new WebClient();
            webClient.Headers.Add("Ocp-Apim-Subscription-Key", "1af9835fbc2a4252acbb62b3bec79c97");
            webClient.Headers.Add(HttpRequestHeader.Accept, "application/json");
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");

            // Serialize the sentiment request with JsonConvert.SerializeObject
            string requestJson = JsonConvert.SerializeObject(sentimentRequest);
            // Turn the request into bytes using Encoding.UTF8.GetBytes
            byte[] requestBytes = Encoding.UTF8.GetBytes(requestJson);
            // Send the request bytes to the url to get the response bytes back, convert it to a string, deserialize it to be able to use it in code
            byte[] response = webClient.UploadData("https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment", requestBytes);
            string sentiments = Encoding.UTF8.GetString(response);
            sentimentResponse = JsonConvert.DeserializeObject<SentimentResponse>(sentiments);
            return sentimentResponse;
        }

        public static JokeResult GetJoke(int numberOfJokes= 1, string limitTo = null)
        {
            var jokes = new JokeResult();  // Joke[numberOfJokes];
            var url = "http://api.icndb.com/jokes/random/";
            using (var webClient = new WebClient())
            {
                if (limitTo != null)
                {
                    url += string.Format("{0}", numberOfJokes);
                } else
                {
                    url += string.Format("{0}?limitTo=[{1}]", numberOfJokes, limitTo);
                }
                byte[] jokeResults = webClient.DownloadData(url);
                var serializer = new JsonSerializer();
                using (var stream = new MemoryStream(jokeResults))
                using(var reader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    jokes = serializer.Deserialize<JokeResult>(jsonReader);
                }
            }
            jokes.Category = limitTo;

            return jokes;
        }

        public static List<JokeResult> GetXJokesForCategories(int x = 1, string categoryString = null)
        {
            // List of lists to hold the joke lists for each category
            var listOfJokeLists = new List<JokeResult>();
            // Split categoryString to get list of categories if not null
            if (categoryString != null)
            {
                var categories = categoryString.Split(new char[] { ',' });
                // Loop through the categories and add the joke lists to the listOfJokeLists
                foreach (var category in categories)
                {
                    listOfJokeLists.Add(GetJoke(x, category));
                }
            } else
            {
                listOfJokeLists.Add(GetJoke(x));
            }
            return listOfJokeLists;
        }
    }
}
