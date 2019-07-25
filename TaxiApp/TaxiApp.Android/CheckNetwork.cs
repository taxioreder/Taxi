using Android.App;
using Android.Content;
using Android.Net;
using TaxiApp.Droid.CustomGeofense;

namespace TaxiApp.Droid
{

    [BroadcastReceiver(Exported = false, DirectBootAware = true)]
    [IntentFilter(new[] { Android.Net.ConnectivityManager.ConnectivityAction, Android.Content.Intent.ActionBootCompleted })]
    public class CheckNetwork : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if(IsOnline(context))
            {
                if (GefenceLocation.gefenceModel == null)
                {
                    if (GefenceLocation.ResetGeofnceModel())
                    {
                        GefenceLocation.UpdateLocation();
                    }
                }
            }
        }

        public bool IsOnline(Context context)
        {
            var cm = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            return cm.ActiveNetworkInfo == null ? false : cm.ActiveNetworkInfo.IsConnected;
        }
    }
}