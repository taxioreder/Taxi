using Plugin.Geofence.Abstractions;

namespace TaxiApp.NewElement.Geofence
{
    public class CrossGeofenceListener : IGeofenceListener
    {
        public void OnAppStarted()
        {
        }

        public void OnError(string error)
        {
        }

        public void OnLocationChanged(GeofenceLocation location)
        {
        }

        public void OnMonitoringStarted(string identifier)
        {
        }

        public void OnMonitoringStopped()
        {
        }

        public void OnMonitoringStopped(string identifier)
        {
        }

        public void OnRegionStateChanged(GeofenceResult result)
        {
        }
    }
}