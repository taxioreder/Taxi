using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using TaxiApp.Models;

namespace TaxiApp.Service
{
    public class OrderGet
    {
        public int ActiveOreder(string token, ref string description, ref List<Order> orders)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Api.Mobile/Order", Method.POST);
                client.Timeout = 20000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
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
                return GetData(content, ref description, ref orders);
            }
        }

        public int OrederMobile(string token, ref string description, ref OrderMobile orderMobile)
        {
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Api.Mobile/OrderMobile", Method.POST);
                client.Timeout = 20000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
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
                return GetData(content, ref description, ref orderMobile);
            }
        }

        private int GetData(string respJsonStr, ref string description, ref OrderMobile orderMobile)
        {
            respJsonStr = respJsonStr.Replace("\\", "");
            respJsonStr = respJsonStr.Remove(0, 1);
            respJsonStr = respJsonStr.Remove(respJsonStr.Length - 1);
            var responseAppS = JObject.Parse(respJsonStr);
            string status = responseAppS.Value<string>("Status");
            if (status == "success")
            {
                orderMobile = JsonConvert.DeserializeObject<OrderMobile>(responseAppS.
                        SelectToken("ResponseStr").ToString());
                description = responseAppS
                    .Value<string>("Description");
                return 3;
            }
            else
            {
                description = responseAppS
                    .Value<string>("Description");
                return 2;
            }
        }

        private int GetData(string respJsonStr, ref string description, ref List<Order> orders)
        {
            respJsonStr = respJsonStr.Replace("\\", "");
            respJsonStr = respJsonStr.Remove(0, 1);
            respJsonStr = respJsonStr.Remove(respJsonStr.Length - 1);
            var responseAppS = JObject.Parse(respJsonStr);
            string status = responseAppS.Value<string>("Status");
            if (status == "success")
            {
                orders = JsonConvert.DeserializeObject<List<Order>>(responseAppS.
                        SelectToken("ResponseStr").ToString());
                description = responseAppS
                    .Value<string>("Description");
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