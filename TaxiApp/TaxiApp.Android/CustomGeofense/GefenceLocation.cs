using System.Collections.Generic;
using Android;
using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Com.Karumi.Dexter;
using Com.Karumi.Dexter.Listener;
using Com.Karumi.Dexter.Listener.Single;
using TaxiApp.Droid.CustomGeofense;
using Xamarin.Forms;

[assembly: Dependency(typeof(GefenceLocation))]
namespace TaxiApp.Droid.CustomGeofense
{
    public class GefenceLocation : AppCompatActivity, IPermissionListener, Service.Geofence.IGeofence
    {
        LocationRequest locationRequest;
        FusedLocationProviderClient FusedLocationProviderClient;
        public static GefenceModel gefenceModel = null;
        MainActivity MainActivity = null;
        public bool IsListenGefence { get; set; }

        public GefenceLocation()
        {
            MainActivity = MainActivity.GetInstance();
        }

        public void OnPermissionDenied(PermissionDeniedResponse p0)
        {
            
        }

        public void OnPermissionGranted(PermissionGrantedResponse p0)
        {
            UpdateLocation();
        }

        private void UpdateLocation()
        {
            LocationReqvest();
            FusedLocationProviderClient = LocationServices.GetFusedLocationProviderClient(MainActivity);
            if (ActivityCompat.CheckSelfPermission(MainActivity.GetInstance(), Manifest.Permission.AccessFineLocation) != Android.Content.PM.Permission.Granted)
            {
                return;
            }
            gefenceModel.PendingIntent = GetPendingIntent();
            FusedLocationProviderClient.RequestLocationUpdates(locationRequest, gefenceModel.PendingIntent);
        }

        private PendingIntent GetPendingIntent()
        {
            Intent intent = new Intent(MainActivity, typeof(LocationLestener));
            intent.SetAction(LocationLestener.ACTION_PROCESS_LOCATIOM);
            return PendingIntent.GetBroadcast(MainActivity, 0, intent, PendingIntentFlags.UpdateCurrent);
        }

        private void LocationReqvest()
        {
            locationRequest = new LocationRequest();
            locationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
            locationRequest.SetInterval(100);
            locationRequest.SetFastestInterval(50);
            var s = locationRequest.SetSmallestDisplacement(1);
        }

        public void OnPermissionRationaleShouldBeShown(PermissionRequest p0, IPermissionToken p1)
        {
            
        }

        public void StartGeofence(int Id, string status, double fromLat, double fromLng, double tooLat, double toLng, double radius)
        {
            if (gefenceModel == null)
            {
                gefenceModel = new GefenceModel();
            }
            gefenceModel.FromLat = fromLat;
            gefenceModel.FromLng = fromLng;
            gefenceModel.ToLat = tooLat;
            gefenceModel.ToLng = toLng;
            gefenceModel.Status = status;
            gefenceModel.Radius = radius;
            gefenceModel.Id = Id;
            Dexter.WithActivity(MainActivity)
                .WithPermission(Manifest.Permission.AccessFineLocation)
                .WithListener(this)
                .Check();
        }

        public void StopGeofence()
        {
            if (gefenceModel != null)
            {
                gefenceModel.PendingIntent.Cancel();
                gefenceModel = null;
            }
        }

        public void ContinueGeofence(string status)
        {
            if(gefenceModel != null)
            {
                gefenceModel.Status = status;
            }
        }
    }
}