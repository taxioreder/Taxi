namespace DBAplication.Model
{
    public class Driver
    {
        public int ID { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public string TokenShope { get; set; }
        public string ZipCod { get; set; }
        public bool IsWork { get; set; }
        public Geolocations geolocations { get; set; }
    }
}