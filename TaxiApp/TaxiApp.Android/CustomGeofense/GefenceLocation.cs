using System;
using System.Collections.Generic;
using System.Linq;
using Android;
using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Com.Karumi.Dexter;
using Com.Karumi.Dexter.Listener;
using Com.Karumi.Dexter.Listener.Single;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Settings;
using RestSharp;
using TaxiApp.Droid.CustomGeofense;
using TaxiApp.Models;
using TaxiApp.Service;
using TaxiApp.Service.Geofence;
using Xamarin.Forms;

[assembly: Dependency(typeof(GefenceLocation))]
namespace TaxiApp.Droid.CustomGeofense
{
    public class GefenceLocation : AppCompatActivity, IPermissionListener, Service.Geofence.IGeofence
    {
        LocationRequest locationRequest;
        FusedLocationProviderClient FusedLocationProviderClient;
        public static GefenceModel gefenceModel = null;
        MainActivity MainActivity = null;

        public bool IsListenGefence { get; set; }

        public GefenceLocation()
        {
            MainActivity = MainActivity.GetInstance();
        }

        public void OnPermissionDenied(PermissionDeniedResponse p0)
        {
            //MainActivity
        }

        public void OnPermissionGranted(PermissionGrantedResponse p0)
        {
            UpdateLocation();
        }

        private void UpdateLocation()
        {
            LocationReqvest();
            FusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(MainActivity);
            if (ActivityCompat.CheckSelfPermission(MainActivity.GetInstance(), Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted)
            {
                return;
            }
            gefenceModel.PendingIntent = GetPendingIntent();
            FusedLocationProviderClient.RequestLocationUpdates(locationRequest, gefenceModel.PendingIntent);
        }

        private PendingIntent GetPendingIntent()
        {
            Intent intent = new Intent(MainActivity, typeof(LocationLestener));
            intent.SetAction(LocationLestener.ACTION_PROCESS_LOCATIOM);
            return PendingIntent.GetBroadcast(MainActivity, 0, intent, PendingIntentFlags.UpdateCurrent);
        }

        private void LocationReqvest()
        {
            locationRequest = new LocationRequest();
            locationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
            locationRequest.SetInterval(100);
            locationRequest.SetFastestInterval(50);
            locationRequest.SetSmallestDisplacement(1);
        }

        public void OnPermissionRationaleShouldBeShown(PermissionRequest p0, IPermissionToken p1)
        {
            
        }

        [Obsolete]
        public void StartGeofence(OrderMobile orderMobile)
        {
            if (gefenceModel == null)
            {
                gefenceModel = new GefenceModel();
            }
            gefenceModel.OrderMobile = orderMobile;
            gefenceModel.OnePointForAddressOrder = orderMobile.OnePointForAddressOrders[0];
            gefenceModel.IsNewOrder = false;
            Dexter.WithActivity(MainActivity)
                .WithPermission(Manifest.Permission.AccessFineLocation)
                .WithListener(this)
                .Check();
        }

        public void StopGeofence()
        {
            if (gefenceModel != null)
            {
                gefenceModel.PendingIntent.Cancel();
                gefenceModel = null;
            }
        }

        public async void ContinueGeofence()
        {
            if (gefenceModel == null && !GefenceLocation.ResetGeofnceModel())
            {
                return;
            }
            else
            {
                UpdateLocation();
            }
            GefenceManager gefenceManager = new GefenceManager(); 
            int index = GefenceLocation.gefenceModel.OrderMobile.OnePointForAddressOrders.FindIndex(one => one == GefenceLocation.gefenceModel.OnePointForAddressOrder);
            if (GefenceLocation.gefenceModel.OrderMobile.OnePointForAddressOrders.Count - 1 != index)
            {
                await gefenceManager.RecurentStatusOrder(GefenceLocation.gefenceModel.OrderMobile.ID, "CompletePoint");
                GefenceLocation.gefenceModel.OnePointForAddressOrder = GefenceLocation.gefenceModel.OrderMobile.OnePointForAddressOrders[index + 1];
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
                client.Timeout = 20000;
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
                if(GetData(content, ref orderMobile) == 3)
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

        public async void EndGeofence()
        {
            if (gefenceModel == null && !GefenceLocation.ResetGeofnceModel())
            {
                return;
            }
            GefenceManager gefenceManager = new GefenceManager();
            int index = GefenceLocation.gefenceModel.OrderMobile.OnePointForAddressOrders.FindIndex(one => one == GefenceLocation.gefenceModel.OnePointForAddressOrder);
            if (GefenceLocation.gefenceModel.OrderMobile.OnePointForAddressOrders.Count - 1 == index)
            {
                await gefenceManager.RecurentStatusOrder(GefenceLocation.gefenceModel.OrderMobile.ID, "NewOrderAndEndOrder");
                if(gefenceModel.PendingIntent != null)
                {
                    gefenceModel.PendingIntent.Cancel();
                }
                gefenceModel = null;
            }
        }
    }
}