using Android.App;
using Android.Content;
using Android.Media;
using Android.Support.V4.App;
using Firebase.Messaging;

namespace TaxiApp.Droid.Alerts
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        public override void OnMessageReceived(RemoteMessage message)
        {
            var body = message.GetNotification();
            SendNotification(body.Body, body.Title, body.ClickAction);
        }

        void SendNotification(string messageBody, string title, string actionClick)
        {
            Intent intent = new Intent(this, typeof(MainActivity));
            const int pendingIntentId = 0;
            PendingIntent.GetActivity(this, pendingIntentId, intent, PendingIntentFlags.OneShot);
            PendingIntent pendingIntent = GetIntentOrder(actionClick);
            var notificationBuilder = new NotificationCompat.Builder(this)
                                      .SetSmallIcon(Resource.Drawable.icon)
                                      .SetContentTitle(title)
                                      .SetContentText(messageBody)
                                      .SetAutoCancel(true)
                                      .SetContentIntent(pendingIntent)
                                      .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.All))
                                      .SetDefaults((int)NotificationDefaults.All)
                                      .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Ringtone))
                                      .SetPriority((int)NotificationPriority.High)
                                      .SetVisibility((int)NotificationVisibility.Public);
            var notificationManager = NotificationManagerCompat.From(this);
            notificationManager.Notify(100, notificationBuilder.Build());
        }

        private PendingIntent GetIntentOrder(string actionClick)
        {
            PendingIntent pendingIntent = null;
            if (actionClick == "No Action")
            {
                pendingIntent = null;
            }
            else if (actionClick == "Order")
            {
                Intent intent = new Intent(this, typeof(MainActivity));
                const int pendingIntentId = 100;
                PendingIntent.GetActivity(this, pendingIntentId, intent, PendingIntentFlags.OneShot);
                pendingIntent = PendingIntent.GetActivity(this, 100, intent, PendingIntentFlags.OneShot);
            }
            return pendingIntent;
        }
    }
}