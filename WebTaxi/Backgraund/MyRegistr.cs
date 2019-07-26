using FluentScheduler;
using WebTaxi.Backgraund.OrderCheck;

namespace WebTaxi.Backgraund
{
    public class MyRegistr : Registry
    {
        public MyRegistr()
        {
            Schedule<CheckOrderFeedBack>().ToRunNow().AndEvery(1).Minutes();
        }
    }
}