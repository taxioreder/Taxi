using Android.App;

namespace TaxiApp.Droid
{
    [Service]
    public class GeofenceService : Android.App.Service
    {
        public override void OnCreate()
        {
            base.OnCreate();
        }

        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            return StartCommandResult.Sticky;
        }

        public override Android.OS.IBinder OnBind(Android.Content.Intent intent)
        {
            return null;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}