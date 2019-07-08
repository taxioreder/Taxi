﻿using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.Widget;
using TaxiApp.Service.Geofence;

namespace TaxiApp.Droid.CustomGeofense
{
    [BroadcastReceiver]
    [IntentFilter(new[] { Intent.ActionBootCompleted }, Priority = (int)IntentFilterPriority.LowPriority)]
    public class LocationLestener : BroadcastReceiver
    {
        public static string ACTION_PROCESS_LOCATIOM = "TaxiApp.Droid.CustomGeofense.UPDATE_LOCATION";

        public override async void OnReceive(Context context, Intent intent)
        {
            if (intent != null)
            {
                GefenceManager gefenceManager = null;
                string action = intent.Action;
                LocationResult locationResult = LocationResult.ExtractResult(intent);
                //Toast.MakeText(Android.App.Application.Context, "Intent", ToastLength.Long).Show();
                if (locationResult != null && action.Equals(ACTION_PROCESS_LOCATIOM))
                {
                    //Toast.MakeText(Android.App.Application.Context, "Location", ToastLength.Long).Show();
                    var lastloc = locationResult.LastLocation;
                    int index = GefenceLocation.gefenceModel.OrderMobile.OnePointForAddressOrders.FindIndex(one => one == GefenceLocation.gefenceModel.OnePointForAddressOrder);
                    if(GefenceLocation.gefenceModel.OrderMobile.OnePointForAddressOrders.Count-1 == index)
                    {
                        if ((GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat - 0.003 < lastloc.Latitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat + 0.00 > lastloc.Latitude)
                           && (GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng - 0.003 < lastloc.Longitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng + 0.003 > lastloc.Longitude) && !GefenceLocation.gefenceModel.IsNewOrder)
                        {
                            GefenceLocation.gefenceModel.IsNewOrder = true;
                            //NewOrder
                        }
                        else if ((GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat - 0.002 < lastloc.Latitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat + 0.002 > lastloc.Latitude)
                            && (GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng - 0.002 < lastloc.Longitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng + 0.002 > lastloc.Longitude) && GefenceLocation.gefenceModel.IsNewOrder)
                        {

                        }
                    }
                    else
                    {
                        if(GefenceLocation.gefenceModel.OnePointForAddressOrder.Type == "Start")
                        {
                            if ((GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat - 0.002 < lastloc.Latitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat + 0.002 > lastloc.Latitude)
                            && (GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng - 0.002 < lastloc.Longitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng + 0.002 > lastloc.Longitude) && GefenceLocation.gefenceModel.IsNewOrder)
                            {
                                GefenceLocation.gefenceModel.OnePointForAddressOrder = GefenceLocation.gefenceModel.OrderMobile.OnePointForAddressOrders[index + 1];
                            }
                        }
                        else if(GefenceLocation.gefenceModel.OnePointForAddressOrder.Type == "End")
                        {
                            if ((GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat - 0.0015 < lastloc.Latitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat + 0.0015 > lastloc.Latitude)
                            && (GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng - 0.0015 < lastloc.Longitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng + 0.0015 > lastloc.Longitude) && GefenceLocation.gefenceModel.IsNewOrder)
                            {
                                GefenceLocation.gefenceModel.OnePointForAddressOrder = GefenceLocation.gefenceModel.OrderMobile.OnePointForAddressOrders[index + 1];
                            }
                        }
                    }

                    //if(GefenceLocation.gefenceModel != null && GefenceLocation.gefenceModel.Status == "From")
                    //{
                    //    //Toast.MakeText(Android.App.Application.Context, "From1", ToastLength.Long).Show();
                    //    if ((GefenceLocation.gefenceModel.FromLat - GefenceLocation.gefenceModel.Radius < lastloc.Latitude && GefenceLocation.gefenceModel.FromLat + GefenceLocation.gefenceModel.Radius > lastloc.Latitude)
                    //        &&(GefenceLocation.gefenceModel.FromLng - GefenceLocation.gefenceModel.Radius < lastloc.Longitude && GefenceLocation.gefenceModel.FromLng + GefenceLocation.gefenceModel.Radius > lastloc.Longitude))
                    //    {
                    //        //Toast.MakeText(Android.App.Application.Context, "From", ToastLength.Long).Show();
                    //        gefenceManager = new GefenceManager();
                    //        GefenceLocation.gefenceModel.Status = "Order";
                    //        await gefenceManager.RecurentStatusOrder("DriveTo", GefenceLocation.gefenceModel.Id);
                    //        gefenceManager.GoDriveTo(GefenceLocation.gefenceModel.ToLat, GefenceLocation.gefenceModel.ToLng);
                    //    }
                    //}
                    //else if (GefenceLocation.gefenceModel != null && GefenceLocation.gefenceModel.Status == "Order")
                    //{
                    //    //Toast.MakeText(Android.App.Application.Context, "Order1", ToastLength.Long).Show();
                    //    if ((GefenceLocation.gefenceModel.ToLat - (GefenceLocation.gefenceModel.Radius + 0.0015) < lastloc.Latitude && GefenceLocation.gefenceModel.ToLat + (GefenceLocation.gefenceModel.Radius + 0.003) > lastloc.Latitude)
                    //        && (GefenceLocation.gefenceModel.ToLng - (GefenceLocation.gefenceModel.Radius + 0.0015) < lastloc.Longitude && GefenceLocation.gefenceModel.ToLng + (GefenceLocation.gefenceModel.Radius + 0.003) > lastloc.Longitude))
                    //    {
                    //        //Toast.MakeText(Android.App.Application.Context, "Order", ToastLength.Long).Show();
                    //        gefenceManager = new GefenceManager();
                    //        GefenceLocation.gefenceModel.Status = "To";
                    //        await gefenceManager.RecurentStatusOrder("Next", GefenceLocation.gefenceModel.Id);
                    //    }
                    //}
                    //else if(GefenceLocation.gefenceModel != null && GefenceLocation.gefenceModel.Status == "To")
                    //{
                    //    //Toast.MakeText(Android.App.Application.Context, "To1", ToastLength.Long).Show();
                    //    if ((GefenceLocation.gefenceModel.ToLat - (GefenceLocation.gefenceModel.Radius + 0.0025) < lastloc.Latitude && GefenceLocation.gefenceModel.ToLat + (GefenceLocation.gefenceModel.Radius + 0.002) > lastloc.Latitude)
                    //        && (GefenceLocation.gefenceModel.ToLng - (GefenceLocation.gefenceModel.Radius + 0.0025) < lastloc.Longitude && GefenceLocation.gefenceModel.ToLng + (GefenceLocation.gefenceModel.Radius + 0.002) > lastloc.Longitude))
                    //    {
                    //        //Toast.MakeText(Android.App.Application.Context, "To", ToastLength.Long).Show();
                    //        gefenceManager = new GefenceManager();
                    //        GefenceLocation.gefenceModel.Status = "None";
                    //        await gefenceManager.RecurentStatusOrder("NewNext", GefenceLocation.gefenceModel.Id);
                    //        GefenceLocation.gefenceModel.PendingIntent.Cancel();
                    //        GefenceLocation.gefenceModel = null;
                    //        if (MainActivity.GetInstance() == null)
                    //        {
                    //            MainActivity mainActivity = new MainActivity();
                    //            mainActivity.Intent = new Intent(context, typeof(MainActivity));
                    //            mainActivity.Intent.AddFlags(ActivityFlags.NewTask);
                    //            context.StartActivity(mainActivity.Intent);
                    //        }
                    //        else
                    //        {
                    //            context.StartActivity(MainActivity.GetInstance().Intent);
                    //        }
                    //    }
                    //}
                }
            }
        }
    }
}