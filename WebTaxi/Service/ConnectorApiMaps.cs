using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;

namespace WebTaxi.Service
{
    public class ConnectorApiMaps
    {
        public Model.Location GetGetLonAndLanToAddress(string address)
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

        public int GetGetDuration(string addressFrom, string addressTo)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient("https://maps.googleapis.com");
                RestRequest request = new RestRequest($"maps/api/distancematrix/json?origins={addressFrom}&destinations={addressTo}&language=En&key=AIzaSyBg0nsyrrsmGyw9Iiw0TOu4HI6o8Jt1jHU", Method.GET);
                request.Timeout = 200000;
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception)
            {
                return 0;
            }
            if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return 0;
            }
            else
            {
                return GetData1(content);
            }
        }

        private int GetData1(string respJsonStr)
        {
            int duration = 0;
            var responseAppS = JObject.Parse(respJsonStr);
            var status = responseAppS.Value<string>("status");
            if (status == "OK")
            {
                duration = responseAppS.GetValue("rows").First.Value<JToken>("elements").First.Value<JToken>("duration").Value<int>("value");
            }
            else
            {

            }

            return duration;
        }

        private Model.Location GetData(string respJsonStr)
        {
            Model.Location location = null;
            var responseAppS = JObject.Parse(respJsonStr);
            var status = responseAppS.Value<string>("status");
            if (status == "OK")
            {
                var stepJson = responseAppS.GetValue("results").First.Value<JToken>("geometry").Value<JToken>("location").ToString();
                location = JsonConvert.DeserializeObject<Model.Location>(stepJson);
            }
            else
            {

            }

            return location;
        }
    }
}
