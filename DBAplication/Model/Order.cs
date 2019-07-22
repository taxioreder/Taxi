namespace DBAplication.Model
{
    public class Order
    {
        public int ID { get; set; }
        public string CurrentStatus { get; set; }
        public string CurrentOrder { get; set; }
        public string NoName { get; set; }
        public string NoName1 { get; set; }
        public string NameCustomer { get; set; }
        public string Phone { get; set; }
        public string Date { get; set; }
        public string NoName2 { get; set; }
        public string TimeOfPickup { get; set; }
        public string TimeOfAppointment { get; set; }
        public string FromAddress { get; set; }
        public int FromZip { get; set; }
        public string ToAddress { get; set; }
        public int ToZip { get; set; }
        public string Milisse { get; set; }
        public string Price { get; set; }
        public string NoName3 { get; set; }
        public string NoName4 { get; set; }
        public string NoName5 { get; set; }
        public string NoName6 { get; set; }
        public int CountCustomer { get; set; }
        public string Comment { get; set; }
        public string FeedBack { get; set; } = "OK";
        public bool isValid { get; set; } = true;
        public bool isAccept { get; set; }
        public bool IsVisableAccept { get; set; }
        public Driver Driver { get; set; }
    }
}