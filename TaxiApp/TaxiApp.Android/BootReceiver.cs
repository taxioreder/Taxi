using Android.App;
using Android.Content;
using Android.Widget;

namespace TaxiApp.Droid
{
    [BroadcastReceiver]
    public class BootReceiver : BroadcastReceiver
    {
        
		static readonly string TAG = "BootBroadcastReceiver";

        public override void OnReceive(Context context, Intent intent)
        {
            Toast.MakeText(context, "EEE Boy", ToastLength.Long).Show();
        }
    }
}