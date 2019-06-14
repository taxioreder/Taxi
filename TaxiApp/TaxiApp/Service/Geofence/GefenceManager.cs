using Plugin.Settings;
using RestSharp;
using System;
using System.Threading.Tasks;
using TaxiApp.Service.Net;
using Xamarin.Essentials;

namespace TaxiApp.Service.Geofence
{
    public class GefenceManager
    {
        public async void GoDriveTo(double lat, double lng)
        {
            var options = new MapLaunchOptions { Name = "1", NavigationMode = Xamarin.Essentials.NavigationMode.Driving };
            await Map.OpenAsync(lat, lng, options);
        }

        public async Task RecurentStatusOrder(string status, int idOrder)
        {
            IRestResponse response = null;
            string content = null;
            await Task.Run(() => Utils.CheckNet());
            if (App.isNetwork)
            {
                try
                {
                    string token = CrossSettings.Current.GetValueOrDefault("Token", "");
                    RestClient client = new RestClient(Config.BaseReqvesteUrl);
                    RestRequest request = new RestRequest("Api.Mobile/Order/RecurentOrderDrive", Method.POST);
                    client.Timeout = 10000;
                    request.AddHeader("Accept", "application/json");
                    request.AddParameter("token", token);
                    request.AddParameter("status", status);
                    request.AddParameter("idorder", idOrder);
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