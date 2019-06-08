using Xamarin.Essentials;

namespace TaxiApp.Service.Geofence
{
    public class GefenceManager
    {
        public async void GoDriveTo(double lat, double lng)
        {
            var options = new MapLaunchOptions { Name = "2", NavigationMode = Xamarin.Essentials.NavigationMode.Driving };
            await Map.OpenAsync(lat, lng, options);
        }
    }
}
