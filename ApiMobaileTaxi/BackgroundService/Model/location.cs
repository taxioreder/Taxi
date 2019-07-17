namespace ApiMobaileTaxi.BackgroundService.DriverManager
{
    public class location
    {
        public string ID { get; set; }
        public string PickuoTime { get; set; }
        public string ApiniTime { get; set; }
        public int CountCusstomer { get; set; }
        public string Date { get; set; }
        public string Type { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public double latE { get; set; }
        public double lngE { get; set; }

        public location(double lat, double lng)
        {
            this.lng = lng;
            this.lat = lat;
        }
    }
}