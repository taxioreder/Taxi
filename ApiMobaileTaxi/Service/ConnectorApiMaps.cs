using ApiMobaileTaxi.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;

namespace ApiMobaileTaxi.Service
{
    public class ConnectorApiMaps
    {
        public location GetGetLonAndLanToAddress(string address)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient("https://maps.googleapis.com");
                RestRequest request = new RestRequest($"maps/api/geocode/json?address={address}&sensor=true&key=AIzaSyBg0nsyrrsmGyw9Iiw0TOu4HI6o8Jt1jHU", Method.GET);
                request.Timeout = 200000;
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception)
            {
                return null;
            }
            if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            else
            {
                return GetData(content);
            }
        }

        private location GetData(string respJsonStr)
        {
            location location = null;
            var responseAppS = JObject.Parse(respJsonStr);
            var status = responseAppS.Value<string>("status");
            if (status == "OK")
            {
                var stepJson = responseAppS.GetValue("results").First.Value<JToken>("geometry").Value<JToken>("location").ToString();
                location = JsonConvert.DeserializeObject<location>(stepJson);
            }
            else
            {

            }

            return location;
        }
    }
}