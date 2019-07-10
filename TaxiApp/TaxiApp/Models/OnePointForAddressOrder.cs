using Xamarin.Forms;

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
        public string Status { set; get; }
        public Color Color
        {
            get
            {
                Color color = Color.Default;
                if(Status == "DriveFromPoint")
                {
                    color = Color.Blue;
                }
                else if(Status == "CompletePoint")
                {
                    color = Color.Silver;
                }
                else if(Status == null || Status == "")
                {
                    color = Color.GreenYellow;
                }
                return color;
            }
        }
    }
}