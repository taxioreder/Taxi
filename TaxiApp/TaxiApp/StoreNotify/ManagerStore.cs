using Plugin.Settings;
using RestSharp;
using System;
using TaxiApp.Service;

namespace TaxiApp.StoreNotify
{
    public class ManagerStore
    {
        public void SendTokenStore(string tokenStore)
        {
            IRestResponse response = null;
            string content = null;
            if (App.isAvtorization)
            {
                try
                {
                    string token = CrossSettings.Current.GetValueOrDefault("Token", "");
                    RestClient client = new RestClient(Config.BaseReqvesteUrl);
                    RestRequest request = new RestRequest("Mobile/tokenStore/Save", Method.POST);
                    request.AddHeader("Accept", "application/json");
                    request.Parameters.Clear();
                    request.AddParameter("token", token);
                    request.AddParameter("tokenStore", tokenStore);
                    response = client.Execute(request);
                    content = response.Content;
                }
                catch (Exception)
                {
                }
            }
        }
    }
}