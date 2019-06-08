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

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent != null)
            {
                GefenceManager gefenceManager = null;
                string action = intent.Action;
                LocationResult locationResult = LocationResult.ExtractResult(intent);
                if (locationResult != null && action.Equals(ACTION_PROCESS_LOCATIOM))
                {
                    var lastloc = locationResult.LastLocation;
                    if(GefenceLocation.gefenceModel != null && GefenceLocation.gefenceModel.Status == "From")
                    {
                        if((GefenceLocation.gefenceModel.FromLat - GefenceLocation.gefenceModel.Radius < lastloc.Latitude && GefenceLocation.gefenceModel.FromLat + GefenceLocation.gefenceModel.Radius > lastloc.Latitude)
                            &&(GefenceLocation.gefenceModel.FromLng - GefenceLocation.gefenceModel.Radius < lastloc.Longitude && GefenceLocation.gefenceModel.FromLng + GefenceLocation.gefenceModel.Radius > lastloc.Longitude))
                        {
                            gefenceManager = new GefenceManager();
                            GefenceLocation.gefenceModel.Status = "To";
                            gefenceManager.GoDriveTo(GefenceLocation.gefenceModel.ToLat, GefenceLocation.gefenceModel.ToLng);
                        }
                    }
                    else if(GefenceLocation.gefenceModel != null && GefenceLocation.gefenceModel.Status == "To")
                    {
                        if ((GefenceLocation.gefenceModel.ToLat - GefenceLocation.gefenceModel.Radius < lastloc.Latitude && GefenceLocation.gefenceModel.ToLat + GefenceLocation.gefenceModel.Radius > lastloc.Latitude)
                            && (GefenceLocation.gefenceModel.ToLng - GefenceLocation.gefenceModel.Radius < lastloc.Longitude && GefenceLocation.gefenceModel.ToLng + GefenceLocation.gefenceModel.Radius > lastloc.Longitude))
                        {
                            GefenceLocation.gefenceModel.Status = "None";
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
                    }
                }
            }
        }
    }
}