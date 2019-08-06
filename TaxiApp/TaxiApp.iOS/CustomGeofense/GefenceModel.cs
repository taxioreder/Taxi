using CoreLocation;
using TaxiApp.Models;

namespace TaxiApp.iOS.CustomGeofense
{
    public class GefenceModel
    {
        public int Id { get; set; }
        public OrderMobile OrderMobile { get; set; }
        public CLLocationManager LocMgr { get; set; }
        public OnePointForAddressOrder OnePointForAddressOrder { get; set; }
        public bool IsNewOrder { get; set; }
    }
}
