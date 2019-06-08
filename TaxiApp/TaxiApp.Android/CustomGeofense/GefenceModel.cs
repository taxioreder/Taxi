using Android.App;

namespace TaxiApp.Droid.CustomGeofense
{
    public class GefenceModel
    {
        public string Id { get; set; }
        public PendingIntent PendingIntent { get; set; }
        public double FromLat { get; set; }
        public double FromLng { get; set; }
        public double ToLat { get; set; }
        public double ToLng { get; set; }
        public double Radius { get; set; }
        public string Status { get; set; }
    }
}