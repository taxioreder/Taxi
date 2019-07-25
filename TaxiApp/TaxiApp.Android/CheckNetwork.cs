using Android.App;
using Android.Content;
using Android.Net;
using TaxiApp.Droid.CustomGeofense;

namespace TaxiApp.Droid
{

    [BroadcastReceiver(Exported = false, DirectBootAware = true)]
    [IntentFilter(new[] { "android.intent.action.CONNECTIVITY_CHANGE" })]
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
            var cm = (ConnectivityManager)context.GetSystemService(this.Class);
            return cm.ActiveNetworkInfo == null ? false : cm.ActiveNetworkInfo.IsConnected;
        }
    }
}