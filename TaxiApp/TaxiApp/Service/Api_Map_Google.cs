using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using TaxiApp.Models;

namespace TaxiApp.Service
{
    public class Api_Map_Google
    {
        //https://maps.googleapis.com/maps/api/geocode/json?address=QUINCY,494,WILLARDST,APT3,02169&key=AIzaSyBg0nsyrrsmGyw9Iiw0TOu4HI6o8Jt1jHU
        public bool GetGetLonAndLanToAddress(string address, ref location location)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient("https://maps.googleapis.com");
                RestRequest request = new RestRequest($"maps/api/geocode/json?address={address}&key=AIzaSyBg0nsyrrsmGyw9Iiw0TOu4HI6o8Jt1jHU", Method.GET);
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception)
            {
                return false;
            }
            if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
            else
            {
               return GetData(content, ref location);
            }
        }

        private bool GetData(string respJsonStr, ref location location)
        {
            bool isReqvest = false;
            var responseAppS = JObject.Parse(respJsonStr);
            var status = responseAppS.Value<string>("status");
            if (status == "OK")
            {
                var stepJson = responseAppS.GetValue("results").First.Value<JToken>("geometry").Value<JToken>("location").ToString();
                location = JsonConvert.DeserializeObject<location>(stepJson);
                isReqvest = true;
            }
            else
            {
                isReqvest = false;
            }
            return isReqvest;
        }
    }
}