using Android.App;
using TaxiApp.Models;

namespace TaxiApp.Droid.CustomGeofense
{
    public class GefenceModel
    {
        public int Id { get; set; }
        public PendingIntent PendingIntent { get; set; }
        public OrderMobile OrderMobile { get; set; }
        public OnePointForAddressOrder OnePointForAddressOrder { get; set; }
        public bool IsNewOrder { get; set; }
    }
}