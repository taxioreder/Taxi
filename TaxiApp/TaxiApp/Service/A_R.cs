using RestSharp;
using System;
using Newtonsoft.Json.Linq;

namespace TaxiApp.Service
{
    class A_R
    {
        public int Avthorization(string login, string password, ref string description, ref string token)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Api.Mobile/Avtorization", Method.POST);
                request.AddHeader("Accept", "application/json");
                client.Timeout = 20000;
                request.AddParameter("email", login);
                request.AddParameter("password", password);
                response = client.Execute(request);
                content = response.Content;
            }
            catch (Exception)
            {
                return 4;
            }
            if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return 4;
            }
            else
            {
                return GetData(content, ref description, ref token); ;
            }
        }

        private int GetData(string respJsonStr, ref string description, ref string token)
        {
            respJsonStr = respJsonStr.Replace("\\", "");
            respJsonStr = respJsonStr.Remove(0, 1);
            respJsonStr = respJsonStr.Remove(respJsonStr.Length - 1);
            var responseAppS = JObject.Parse(respJsonStr);
            string status = responseAppS.Value<string>("Status");
            if (status == "success")
            {
                token = responseAppS
                    .Value<string>("ResponseStr");
                return 3;
            }
            else
            {
                description = responseAppS
                    .Value<string>("Description");
                return 2;
            }
        }
    }
}