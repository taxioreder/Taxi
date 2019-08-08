using Android.App;
using Firebase.Iid;
using TaxiApp.StoreNotify;

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