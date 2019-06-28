using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Rg.Plugins.Popup.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TaxiApp.Service.Net
{
    public class Utils
    {
        private static bool isAlRedy = false;

        [Obsolete]
        public static async Task CheckNet()
        {
            var sync = SynchronizationContext.Current;
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Api.Mobile/Net", Method.GET);
                request.AddHeader("Accept", "application/json");
                client.Timeout = 20000;
                response = client.Execute(request);
                content = response.Content;
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (content == "" || response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        if (App.isNetwork)
                        {
                            App.isNetwork = false;
                            if (App.isStart)
                            {
                                //await PopupNavigation.PushAsync(new Errror("Not Network", null));
                            }
                            else
                            {
                                //DependencyService.Get<IToast>().ShowMessage("Not Network");
                            }
                        }
                        else if (!isAlRedy)
                        {
                            isAlRedy = true;
                            if (App.isStart)
                            {
                                //await PopupNavigation.PushAsync(new Errror("Not Network", null));
                            }
                            else
                            {
                                //DependencyService.Get<IToast>().ShowMessage("Not Network");
                            }
                            RefreshIsAlRed();
                        }
                    }
                    else
                    {
                        bool isCheck = false;
                        string description = null;
                        GetData(content, ref isCheck, ref description);
                        if (!isCheck)
                        {
                            if (App.isNetwork)
                            {
                                App.isNetwork = false;
                                if (App.isStart)
                                {
                                    //await PopupNavigation.PushAsync(new Errror(description, null));
                                }
                                else
                                {
                                   // DependencyService.Get<IToast>().ShowMessage(description);
                                }
                            }
                            else if (!isAlRedy)
                            {
                                isAlRedy = true;
                                if (App.isStart)
                                {
                                    //await PopupNavigation.PushAsync(new Errror(description, null));
                                }
                                else
                                {
                                    //DependencyService.Get<IToast>().ShowMessage(description);
                                }
                                RefreshIsAlRed();
                            }
                        }
                        else
                        {
                            App.isNetwork = true;
                        }
                    }
                });
            }
            catch (Exception e)
            {

            }
        }

        private static void RefreshIsAlRed()
        {
            Task.Run(() => 
            {
                Thread.Sleep(3000);
                isAlRedy = false;
            });
        }

        private static void GetData(string respJsonStr, ref bool isCheck, ref string description)
        {
            respJsonStr = respJsonStr.Replace("\\", "");
            respJsonStr = respJsonStr.Remove(0, 1);
            respJsonStr = respJsonStr.Remove(respJsonStr.Length - 1);
            var responseAppS = JObject.Parse(respJsonStr);
            string status = responseAppS.Value<string>("Status");
            if (status == "success")
            {
                isCheck = Convert.ToBoolean((responseAppS.
                        SelectToken("ResponseStr").ToString()));
                description = JsonConvert.DeserializeObject<string>(responseAppS.
                        SelectToken("Description").ToString());
            }
            else
            {
                isCheck = false;
                description = "Not Network";
            }
        }
    }
}