using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.OS;
using Xamarin.Forms.Platform.Android;
using Firebase;
using Plugin.FirebasePushNotification;
using Plugin.Permissions;
using System;

namespace TaxiApp.Droid
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        static MainActivity Instance = null;

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(bundle);
            Rg.Plugins.Popup.Popup.Init(this, bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, bundle);
            FirebaseApp.InitializeApp(Android.App.Application.Context);
            FirebasePushNotificationManager.ProcessIntent(this, Intent);
            LoadApplication(new App());
            Instance = this;
        }

        public static MainActivity GetInstance()
        {
            return Instance;
        }


        //PowerManager powerManager = (PowerManager)GetSystemService(PowerService);
        //    if (Build.VERSION.SdkInt >= Build.VERSION_CODES.KitkatWatch)
        //    {
        //        isScreenOn = powerManager.IsInteractive;
        //    }
        //    else
        //    {
        //        isScreenOn = powerManager.IsScreenOn;
        //    }
        //    if (!isScreenOn)
        //    { 
        //        // The screen has been locked // do stuff... 
        //    }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}