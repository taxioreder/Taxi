namespace TaxiApp.Service.Geofence
{
    public interface IGeofence
    {
        void StartGeofence(int id, double fromLat, double fromLng, double tooLat, double toLng, double radius);
        void StopGeofence();
    }
}