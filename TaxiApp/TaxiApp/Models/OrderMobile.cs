using System.Collections.Generic;

namespace TaxiApp.Models
{
    public class OrderMobile
    {
        public int ID { get; set; }
        public string IdDriver { get; set; }
        public string Status { get; set; }
        public List<Order> Orders { get; set; }
        public List<OnePointForAddressOrder> OnePointForAddressOrders { get; set; }
        public bool IsVisableStart
        { 
            get
            {
                bool isVisableStart = false;
                if(Status == "New")
                {
                    isVisableStart = true;
                }
                return isVisableStart;
            }
        }
        public bool IsVisableContinue
        {
            get
            {
                bool isVisableContinue = false;
                if (OnePointForAddressOrders[OnePointForAddressOrders.Count - 2].Status == "DriveFromPoint")
                {
                    isVisableContinue = true;
                }
                return isVisableContinue;
            }
        }
        public bool IsVisableEnd
        {
            get
            {
                bool isVisableEnd = false;
                if (OnePointForAddressOrders[OnePointForAddressOrders.Count - 2].Status == "CompletePoint")
                {
                    isVisableEnd = true;
                }
                return isVisableEnd;
            }
        }
    }
}