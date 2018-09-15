using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace TurnOnLight.Analyzer
{
    public class WebClientLuis
    {
        private const string LuisUrl = " YOUR LUIS ENDPOINT HERE ";
       
        
        private static async Task<string> Get(string url)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(new Uri(url));
            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }

        private static async Task<string> Post(string url, string key, string value)
        {
            HttpClient client = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>(key, value)
            });
            HttpResponseMessage response = await client.PostAsync(new Uri(url), content);
            var responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }

        public static async Task<Analyzer> Order(string sentence)
        {
            var result = new Analyzer
            {
                Entity = Entity.AllLights,
                Intent = Intent.None
            };
            try
            {
                var queryUrl = LuisUrl + sentence;
                var resultJson = await Get(queryUrl);

                //convert JSON in comands
                JsonObject obj = JsonObject.Parse(resultJson);
                var intent = obj["topScoringIntent"].GetObject()["intent"].GetString();
                switch (intent)
                {
                    case "LightOn":
                        result.Intent = Intent.LightOn;
                        break;
                    case "LightOff":
                        result.Intent = Intent.LightOff;
                        break;
                    default:
                        break;
                }

                var entity = obj["entities"];
                if (entity.GetArray().Any())
                {
                    var entityLight = obj["entities"].GetArray()[0].GetObject()["type"].ToString().Replace("\"", "");
                    switch (entityLight)
                    {
                        case "AllLight":
                            result.Entity = Entity.AllLights;
                            break;
                        case "Green":
                            result.Entity = Entity.Green;
                            break;
                        case "Yellow":
                            result.Entity = Entity.Yellow;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                //TODO
            }

            return result;
        }
    }
}
