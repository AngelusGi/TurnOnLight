using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace TurnOnLight
{
    public class WebClientLuis
    {
        private const string LuisUrl = " YOUR LUIS ENDPOINT HERE ";


        private static async Task<string> Get(string url)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(new Uri(url));
            string responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }

        private static async Task<string> Post(string url, string key, string value)
        {
            HttpClient client = new HttpClient();
            FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>(key, value)
            });
            HttpResponseMessage response = await client.PostAsync(new Uri(url), content);
            string responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }

        public static async Task<Analyzer> Order(string sentence)
        {
            Analyzer result = new Analyzer
            {
                Entity = Entity.AllLights,
                Intent = Intent.None
            };
            try
            {
                string queryUrl = LuisUrl + sentence;
                string resultJson = await Get(queryUrl);

                //convert JSON in comands
                JsonObject obj = JsonObject.Parse(resultJson);
                string intent = obj["topScoringIntent"].GetObject()["intent"].GetString();
                switch (intent)
                {
                    case "LightOn":
                        result.Intent = Intent.LightOn;
                        break;
                    case "LightOff":
                        result.Intent = Intent.LightOff;
                        break;
                }

                IJsonValue entity = obj["entities"];
                if (entity.GetArray().Any())
                {
                    string entityLight = obj["entities"].GetArray()[0].GetObject()["type"].ToString().Replace("\"", "");
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
            catch (Exception)
            {
                //TODO
            }

            return result;
        }
    }
}
