using System.Collections.Generic;

namespace DBAplication.Model
{
    public class OrderMobile
    {
        public int ID { get; set; }
        public string IdDriver { get; set; }
        public string Status { get; set; }
        public string StatusDrive { get; set; }
        public List<Order> Orders { get; set; }
        public List<OnePointForAddressOrder> OnePointForAddressOrders { get; set; }
    }
}