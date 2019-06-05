using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Plugin.Geofence;
using TaxiApp.NewElement.Geofence;

namespace TaxiApp.Droid
{
    [Application]
    public class GeofenceAppStarter : Application
    {
        public static Context AppContext;

        public GeofenceAppStarter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        { }

        public override void OnCreate()
        {
            base.OnCreate();
            AppContext = this.ApplicationContext;
            CrossGeofence.Initialize<CrossGeofenceListener>();
            CrossGeofence.GeofenceListener.OnAppStarted();
            StartService();
        }

        public static void StartService()
        {
            AppContext.StartService(new Intent(AppContext, typeof(GeofenceService)));

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Kitkat)
            {

                PendingIntent pintent = PendingIntent.GetService(AppContext, 0, new Intent(AppContext, typeof(GeofenceService)), 0);
                AlarmManager alarm = (AlarmManager)AppContext.GetSystemService(Context.AlarmService);
                alarm.Cancel(pintent);
            }
        }

        public static void StopService()
        {
            AppContext.StopService(new Intent(AppContext, typeof(GeofenceService)));
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Kitkat)
            {
                PendingIntent pintent = PendingIntent.GetService(AppContext, 0, new Intent(AppContext, typeof(GeofenceService)), 0);
                AlarmManager alarm = (AlarmManager)AppContext.GetSystemService(Context.AlarmService);
                alarm.Cancel(pintent);
            }
        }
    }
}