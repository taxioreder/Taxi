using Android.App;
using Firebase.Iid;
using TaxiApp.Droid.Alerts;
using TaxiApp.StoreNotify;

[assembly: Xamarin.Forms.Dependency(typeof(FirebaseIIDService))]
namespace TaxiApp.Droid.Alerts
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class FirebaseIIDService : FirebaseInstanceIdService, IStore
    {
        public override void OnTokenRefresh()
        {
            ManagerStore managerStore = new ManagerStore();
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            managerStore.SendTokenStore(refreshedToken);
        }
    }
}