using Android.App;
using Android.Content;
using Android.Net;
using TaxiApp.Droid.CustomGeofense;

namespace TaxiApp.Droid
{
    [BroadcastReceiver(Exported = false, DirectBootAware = true)]
    [IntentFilter(new[] { "android.intent.action.BOOT_COMPLETED", "android.intent.action.LOCKED_BOOT_COMPLETED" })]
    public class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var cm = (ConnectivityManager)context.GetSystemService(this.Class);
            if (GefenceLocation.gefenceModel == null && cm.ActiveNetworkInfo.IsConnected)
            {
                if(GefenceLocation.ResetGeofnceModel())
                {
                    GefenceLocation.UpdateLocation();
                }
            }
        }
    }
}