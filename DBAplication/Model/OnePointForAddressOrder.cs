namespace DBAplication.Model
{
    public class OnePointForAddressOrder
    {
        public int ID { get; set; }
        public int IDorder { get; set; }
        public double Lat { set; get; } 
        public double Lng { set; get; }
        public string PTime { set; get; }
        public string Date { set; get; }
        public string Type { set; get; }
        public string Address { set; get; }
        public string Status { set; get; }

        public OnePointForAddressOrder(int iDorder, double lat, double lng, string pTime, string date, string type, string address)
        {
            IDorder = iDorder;
            Lat = lat;
            Lng = lng;
            PTime = pTime;
            Date = date;
            Type = type;
            Address = address;
        }
    }
}