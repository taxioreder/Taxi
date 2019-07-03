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
        public string lat { get; set; }
        public string lng { get; set; }
        public string latE { get; set; }
        public string lngE { get; set; }

        public location(string lat = null, string lng = null)
        {
            this.lng = lng;
            this.lat = lat;
        }
    }
}