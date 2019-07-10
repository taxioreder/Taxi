using Android.App;
using Android.Content;
using Android.Gms.Location;
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
                if (locationResult != null && action.Equals(ACTION_PROCESS_LOCATIOM))
                {
                     gefenceManager = new GefenceManager();
                    var lastloc = locationResult.LastLocation;
                    int index = GefenceLocation.gefenceModel.OrderMobile.OnePointForAddressOrders.FindIndex(one => one == GefenceLocation.gefenceModel.OnePointForAddressOrder);
                    if(GefenceLocation.gefenceModel.OrderMobile.OnePointForAddressOrders.Count-1 == index)
                    {
                        if ((GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat - 0.002 < lastloc.Latitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat + 0.002 > lastloc.Latitude)
                               && (GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng - 0.002 < lastloc.Longitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng + 0.002 > lastloc.Longitude))
                        {
                            if (GefenceLocation.gefenceModel.IsNewOrder)
                            {
                                await gefenceManager.RecurentStatusOrder(GefenceLocation.gefenceModel.OrderMobile.ID, "NewOrder");
                            }
                            else
                            {
                                await gefenceManager.RecurentStatusOrder(GefenceLocation.gefenceModel.OrderMobile.ID, "NewOrderAndEndOrder");
                            }
                            GefenceLocation.gefenceModel.PendingIntent.Cancel();
                            GefenceLocation.gefenceModel = null;
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
                        else if ((GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat - 0.003 < lastloc.Latitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat + 0.0033 > lastloc.Latitude)
                           && (GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng - 0.003 < lastloc.Longitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng + 0.0033 > lastloc.Longitude) && !GefenceLocation.gefenceModel.IsNewOrder)
                        {
                            await gefenceManager.RecurentStatusOrder(GefenceLocation.gefenceModel.OrderMobile.ID, "NewOrder");
                            GefenceLocation.gefenceModel.IsNewOrder = true;
                        }
                    }
                    else
                    {
                        if(GefenceLocation.gefenceModel.OnePointForAddressOrder.Type == "Start")
                        {
                            if ((GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat - 0.0015 < lastloc.Latitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat + 0.0015 > lastloc.Latitude)
                            && (GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng - 0.0015 < lastloc.Longitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng + 0.0015 > lastloc.Longitude))
                            {
                                await gefenceManager.RecurentStatusOrder(GefenceLocation.gefenceModel.OrderMobile.ID, "CompletePoint");
                                GefenceLocation.gefenceModel.OnePointForAddressOrder = GefenceLocation.gefenceModel.OrderMobile.OnePointForAddressOrders[index + 1];
                                gefenceManager.GoDriveTo(GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat, GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng);
                            }
                        }
                        else if(GefenceLocation.gefenceModel.OnePointForAddressOrder.Type == "End")
                        {
                            if ((GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat - 0.002 < lastloc.Latitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lat + 0.002 > lastloc.Latitude)
                            && (GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng - 0.002 < lastloc.Longitude && GefenceLocation.gefenceModel.OnePointForAddressOrder.Lng + 0.002 > lastloc.Longitude))
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