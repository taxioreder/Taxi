namespace TaxiApp.Models
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
    }
}