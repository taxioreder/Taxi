using Plugin.Geofence;
using Plugin.Geofence.Abstractions;
using System;
using System.Threading.Tasks;
using TaxiApp.Service;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TaxiApp.NewElement.Geofence
{
    public class CrossGeofenceListener : IGeofenceListener
    {
        private ManagerTaxi managerTaxi = null;

        public async void OnAppStarted()
        {
            await Task.Run(async () =>
            {
               // CrossGeofence.Current.StopMonitoringAllRegions();
            });
        }

        public async void OnError(string error)
        {
            await Task.Run(async () =>
            {
            });
            //CrossGeofence - You need to enabled Location Services
        }

        public async void OnLocationChanged(GeofenceLocation location)
        {
            await Task.Run(async () =>
            {
            });
        }

        public async void OnMonitoringStarted(string identifier)
        {
            await Task.Run(async () =>
            {
            });
        }

        public async void OnMonitoringStopped()
        {
            await Task.Run(async () =>
            {
            });
        }

        public async void OnMonitoringStopped(string identifier)
        {
            await Task.Run(async () =>
            {
            });
        }

        public void OnRegionStateChanged(GeofenceResult  result)
        {
            Device.BeginInvokeOnMainThread(async() =>
            {
                if (result.TransitionName == "Entered")
                {
                    string[] lanlong = result.RegionId.Split(',');
                    CrossGeofence.Current.StopMonitoring(result.RegionId);
                    var options = new MapLaunchOptions { Name = "1", NavigationMode = NavigationMode.None };
                    await Map.OpenAsync(Convert.ToDouble(lanlong[1]), Convert.ToDouble(lanlong[2]), options);
                }
            });
        }
    }
}