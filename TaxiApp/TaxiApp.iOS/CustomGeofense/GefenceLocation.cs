using System;
using System.Linq;
using System.Net.NetworkInformation;
using CoreLocation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Settings;
using RestSharp;
using TaxiApp.iOS.CustomGeofense;
using TaxiApp.Models;
using TaxiApp.Service;
using TaxiApp.Service.Geofence;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(GefenceLocation))]
namespace TaxiApp.iOS.CustomGeofense
{
    public class GefenceLocation : Service.Geofence.IGeofence
    {
        public static GefenceModel gefenceModel = null;

        public async void ContinueGeofence()
        {
            GefenceManager gefenceManager = new GefenceManager();
            if (true)
            {
                if (gefenceModel == null)
                {
                    if (GefenceLocation.ResetGeofnceModel())
                    {
                        UpdateLocation();
                    }
                    else
                    {
                        return;
                    }
                }
                int index = GefenceLocation.gefenceModel.OrderMobile.OnePointForAddressOrders.FindIndex(one => one == GefenceLocation.gefenceModel.OnePointForAddressOrder);
                if (GefenceLocation.gefenceModel.OrderMobile.OnePointForAddressOrders.Count - 1 != index)
                {
                    await gefenceManager.RecurentStatusOrder(GefenceLocation.gefenceModel.OrderMobile.ID, "CompletePoint");
                    GefenceLocation.gefenceModel.OnePointForAddressOrder = GefenceLocation.gefenceModel.OrderMobile.OnePointForAddressOrders[index + 1];
                }
            }
        }

        public async void EndGeofence()
        {
            GefenceManager gefenceManager = new GefenceManager();
            if (true)
            {
                if (gefenceModel == null)
                {
                    if (GefenceLocation.ResetGeofnceModel())
                    {
                        UpdateLocation();
                    }
                    else
                    {
                        return;
                    }
                }
                int index = GefenceLocation.gefenceModel.OrderMobile.OnePointForAddressOrders.FindIndex(one => one == GefenceLocation.gefenceModel.OnePointForAddressOrder);
                if (GefenceLocation.gefenceModel.OrderMobile.OnePointForAddressOrders.Count - 1 == index)
                {
                    await gefenceManager.RecurentStatusOrder(GefenceLocation.gefenceModel.OrderMobile.ID, "NewOrderAndEndOrder");
                    if (gefenceModel.LocMgr != null)
                    {
                        gefenceModel.LocMgr.StopUpdatingLocation();
                    }
                    gefenceModel = null;
                }
            }
        }

        public void StartGeofence(OrderMobile orderMobile)
        {
            if(gefenceModel == null)
            {
                gefenceModel = new GefenceModel();
            }
            gefenceModel.OrderMobile = orderMobile;
            gefenceModel.OnePointForAddressOrder = orderMobile.OnePointForAddressOrders[0];
            gefenceModel.IsNewOrder = false;
            UpdateLocation();
        }

        public void StopGeofence()
        {
            if (gefenceModel != null)
            {
                gefenceModel.LocMgr.StopUpdatingLocation();
                gefenceModel = null;
            }
        }

        public static void UpdateLocation()
        {
            gefenceModel.LocMgr = new CLLocationManager();
            gefenceModel.LocMgr.PausesLocationUpdatesAutomatically = false;
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                gefenceModel.LocMgr.RequestAlwaysAuthorization();
            }

            if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
            {
                gefenceModel.LocMgr.AllowsBackgroundLocationUpdates = true;
            }

            if (CLLocationManager.LocationServicesEnabled)
            {
                gefenceModel.LocMgr.DesiredAccuracy = 10;
                gefenceModel.LocMgr.LocationsUpdated += LocationsUpdated_Event;
                gefenceModel.LocMgr.StartUpdatingLocation();
            }
        }

        private static async void LocationsUpdated_Event(object sender, CLLocationsUpdatedEventArgs e)
        {
            if(true)
            {
                if (gefenceModel == null)
                {
                    if (!GefenceLocation.ResetGeofnceModel())
                    {
                        return;
                    }
                }
                int index = GefenceLocation.gefenceModel.OrderMobile.OnePointForAddressOrders.FindIndex(one => one == GefenceLocation.gefenceModel.OnePointForAddressOrder);
                GefenceManager gefenceManager = new GefenceManager();
                if (GefenceLocation.gefenceModel.OrderMobile.OnePointForAddressOrders.Count - 1 == index)
                {
                    if ((GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat - 0.00073 < e.Locations[0].Coordinate.Latitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat + 0.00073 > e.Locations[0].Coordinate.Latitude)
                           && (GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng - 0.00073 < e.Locations[0].Coordinate.Longitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng + 0.00073 > e.Locations[0].Coordinate.Longitude))
                    {
                        if (GefenceLocation.gefenceModel.IsNewOrder)
                        {
                            await gefenceManager.RecurentStatusOrder(GefenceLocation.gefenceModel.OrderMobile.ID, "NewOrder");
                        }
                        else
                        {
                            await gefenceManager.RecurentStatusOrder(GefenceLocation.gefenceModel.OrderMobile.ID, "NewOrderAndEndOrder");
                        }
                        gefenceModel.LocMgr.StopUpdatingLocation();
                        gefenceModel = null;
                        //ComponentName receiver = new ComponentName(context, this.Class);
                        //PackageManager pm = context.PackageManager;
                        //pm.SetComponentEnabledSetting(receiver, ComponentEnabledState.Disabled, ComponentEnableOption.DontKillApp);
                        //if (MainActivity.GetInstance() == null)
                        //{
                        //    MainActivity mainActivity = new MainActivity();
                        //    mainActivity.Intent = new Intent(context, typeof(MainActivity));
                        //    mainActivity.Intent.AddFlags(ActivityFlags.NewTask);
                        //    context.StartActivity(mainActivity.Intent);
                        //}
                        //else
                        //{
                        //    context.StartActivity(MainActivity.GetInstance().Intent);
                        //}
                    }
                    else if ((GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat - 0.0022 < e.Locations[0].Coordinate.Latitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat + 0.0022 > e.Locations[0].Coordinate.Latitude)
                       && (GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng - 0.0022 < e.Locations[0].Coordinate.Longitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng + 0.0022 > e.Locations[0].Coordinate.Longitude) && !GefenceLocation.gefenceModel.IsNewOrder)
                    {
                        await gefenceManager.RecurentStatusOrder(GefenceLocation.gefenceModel.OrderMobile.ID, "NewOrder");
                        GefenceLocation.gefenceModel.IsNewOrder = true;
                    }
                }
                else
                {
                    if (GefenceLocation.gefenceModel.OnePointForAddressOrder.Type == "Start")
                    {
                        if ((GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat - 0.00073 < e.Locations[0].Coordinate.Latitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat + 0.00073 > e.Locations[0].Coordinate.Latitude)
                        && (GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng - 0.00073 < e.Locations[0].Coordinate.Longitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng + 0.00073 > e.Locations[0].Coordinate.Longitude))
                        {
                            await gefenceManager.RecurentStatusOrder(GefenceLocation.gefenceModel.OrderMobile.ID, "CompletePoint");
                            GefenceLocation.gefenceModel.OnePointForAddressOrder = GefenceLocation.gefenceModel.OrderMobile.OnePointForAddressOrders[index + 1];
                            gefenceManager.GoDriveTo(GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat, GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng);
                        }
                    }
                    else if (GefenceLocation.gefenceModel.OnePointForAddressOrder.Type == "End")
                    {
                        if ((GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat - 0.00073 < e.Locations[0].Coordinate.Latitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat + 0.00073 > e.Locations[0].Coordinate.Latitude)
                        && (GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng - 0.00073 < e.Locations[0].Coordinate.Longitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng + 0.00073 > e.Locations[0].Coordinate.Longitude))
                        {
                            await gefenceManager.RecurentStatusOrder(GefenceLocation.gefenceModel.OrderMobile.ID, "CompletePoint");
                            GefenceLocation.gefenceModel.OnePointForAddressOrder = GefenceLocation.gefenceModel.OrderMobile.OnePointForAddressOrders[index + 1];
                            gefenceManager.GoDriveTo(GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat, GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng);
                        }
                    }
                }
            }
        }

        public static bool ResetGeofnceModel()
        {
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            OrderMobile orderMobile = null;
            IRestResponse response = null;
            string content = null;
            try
            {
                RestClient client = new RestClient(Config.BaseReqvesteUrl);
                RestRequest request = new RestRequest("Api.Mobile/OrderMobile", Method.POST);
                client.Timeout = 60000;
                request.AddHeader("Accept", "application/json");
                request.AddParameter("token", token);
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
                if (GetData(content, ref orderMobile) == 3)
                {
                    if (gefenceModel == null)
                    {
                        gefenceModel = new GefenceModel();
                    }
                    gefenceModel.OrderMobile = orderMobile;
                    gefenceModel.OnePointForAddressOrder = orderMobile.OnePointForAddressOrders.FirstOrDefault(oP => oP.Status == "DriveFromPoint");
                    gefenceModel.IsNewOrder = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private static int GetData(string respJsonStr, ref OrderMobile orderMobile)
        {
            try
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
                    return 3;
                }
                else
                {
                    return 2;
                }
            }
            catch
            {
                return 2;
            }
        }

        private static bool IsInternetAviable()
        {
            try
            {
                Ping myPing = new Ping();
                String host = "google.com";
                byte[] buffer = new byte[32];
                int timeout = 1000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    } 
}