namespace TaxiApp.Models
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
        public string ToAddress { get; set; }
        public string Milisse { get; set; }
        public string Price { get; set; }
        public string NoName3 { get; set; }
        public string NoName4 { get; set; }
        public string NoName5 { get; set; }
        public string NoName6 { get; set; }
        public string Comment { get; set; }
        public bool IsEnable
        {
            get
            {
                if(CurrentOrder == "Next")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public string ColorOrder
        {
            get
            {
                if (CurrentOrder == "Next")
                {
                    return "#BDBDBD";
                }
                else
                {
                    return "#FFFFFF";
                }
            }
        }

        public string BtnStart
        {
            get
            {
                if (CurrentOrder == "Next")
                {
                    return "";
                }
                else if (CurrentOrder == "New")
                {
                    return "Start";
                }
                else if(CurrentOrder == "DriveFrome")
                {
                    return "Build a route to the client";
                }
                else if (CurrentOrder == "DriveTo")
                {
                    return "Build a route to the end point";
                }
                else if (CurrentOrder == "NewNext")
                {
                    return "Start";
                }
                return "";
            }
        }

        public bool IsVisableCloseOrder
        {
            get
            {
                if (CurrentOrder == "DriveTo")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsVisableContinueOrder
        {
            get
            {
                if (CurrentOrder == "DriveFrome")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsVisableCancelOrder
        {
            get
            {
                return true;
            }
        }
    }
}