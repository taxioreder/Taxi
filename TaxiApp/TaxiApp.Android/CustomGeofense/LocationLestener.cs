using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Location;
using Android.Net;
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
            var cm = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            if (intent != null && (cm.ActiveNetworkInfo == null ? false : cm.ActiveNetworkInfo.IsConnected))
            {
                if (GefenceLocation.gefenceModel == null)
                {
                    if (!GefenceLocation.ResetGeofnceModel())
                    {
                        ComponentName receiver = new ComponentName(context, this.Class);
                        PackageManager pm = context.PackageManager;
                        pm.SetComponentEnabledSetting(receiver, ComponentEnabledState.Disabled, ComponentEnableOption.DontKillApp);
                        return;
                    }
                }
                GefenceManager gefenceManager = null;
                string action = intent.Action;
                LocationResult locationResult = LocationResult.ExtractResult(intent);
                if (locationResult != null && action.Equals(ACTION_PROCESS_LOCATIOM))
                {
                    gefenceManager = new GefenceManager();
                    var lastloc = locationResult.LastLocation;
                    int index = GefenceLocation.gefenceModel.OrderMobile.OnePointForAddressOrders.FindIndex(one => one == GefenceLocation.gefenceModel.OnePointForAddressOrder);
                    if(GefenceLocation.gefenceModel.OrderMobile.OnePointForAddressOrders.Count-1 == index)
                    {
                        if ((GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat - 0.00073 < lastloc.Latitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat + 0.00073 > lastloc.Latitude)
                               && (GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng - 0.00073 < lastloc.Longitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng + 0.00073 > lastloc.Longitude))
                        {
                            if (GefenceLocation.gefenceModel.IsNewOrder)
                            {
                                await gefenceManager.RecurentStatusOrder(GefenceLocation.gefenceModel.OrderMobile.ID, "NewOrder");
                            }
                            else
                            {
                                await gefenceManager.RecurentStatusOrder(GefenceLocation.gefenceModel.OrderMobile.ID, "NewOrderAndEndOrder");
                            }
                            ComponentName receiver = new ComponentName(context, this.Class);
                            PackageManager pm = context.PackageManager;
                            pm.SetComponentEnabledSetting(receiver, ComponentEnabledState.Disabled, ComponentEnableOption.DontKillApp);
                            if (MainActivity.GetInstance() == null)
                            {
                                MainActivity mainActivity = new MainActivity();
                                mainActivity.Intent = new Intent(context, typeof(MainActivity));
                                mainActivity.Intent.AddFlags(ActivityFlags.NewTask);
                                context.StartActivity(mainActivity.Intent);
                            }
                            else
                            {
                                context.StartActivity(MainActivity.GetInstance().Intent);
                            }
                        }
                        else if ((GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat - 0.0022 < lastloc.Latitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat + 0.0022 > lastloc.Latitude)
                           && (GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng - 0.0022 < lastloc.Longitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng + 0.0022 > lastloc.Longitude) && !GefenceLocation.gefenceModel.IsNewOrder)
                        {
                            await gefenceManager.RecurentStatusOrder(GefenceLocation.gefenceModel.OrderMobile.ID, "NewOrder");
                            GefenceLocation.gefenceModel.IsNewOrder = true;
                        }
                    }
                    else
                    {
                        if(GefenceLocation.gefenceModel.OnePointForAddressOrder.Type == "Start")
                        {
                            if ((GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat - 0.00073 < lastloc.Latitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat + 0.00073 > lastloc.Latitude)
                            && (GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng - 0.00073 < lastloc.Longitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng + 0.00073 > lastloc.Longitude))
                            {
                                await gefenceManager.RecurentStatusOrder(GefenceLocation.gefenceModel.OrderMobile.ID, "CompletePoint");
                                GefenceLocation.gefenceModel.OnePointForAddressOrder = GefenceLocation.gefenceModel.OrderMobile.OnePointForAddressOrders[index + 1];
                                gefenceManager.GoDriveTo(GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat, GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng);
                            }
                        }
                        else if(GefenceLocation.gefenceModel.OnePointForAddressOrder.Type == "End")
                        {
                            if ((GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat - 0.00073 < lastloc.Latitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat + 0.00073 > lastloc.Latitude)
                            && (GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng - 0.00073 < lastloc.Longitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng + 0.00073 > lastloc.Longitude))
                            {
                                await gefenceManager.RecurentStatusOrder(GefenceLocation.gefenceModel.OrderMobile.ID, "CompletePoint");
                                GefenceLocation.gefenceModel.OnePointForAddressOrder = GefenceLocation.gefenceModel.OrderMobile.OnePointForAddressOrders[index + 1];
                                gefenceManager.GoDriveTo(GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat, GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng);
                            }
                        }
                    }
                }
            }
        }
    }
}