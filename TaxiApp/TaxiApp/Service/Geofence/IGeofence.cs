using TaxiApp.Models;

namespace TaxiApp.Service.Geofence
{
    public interface IGeofence
    {
        void StartGeofence(OrderMobile orderMobile);
        void StopGeofence();
        void ContinueGeofence(string status);
    }
}