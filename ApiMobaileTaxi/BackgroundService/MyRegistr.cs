using ApiMobaileTaxi.BackgroundService.DriverManager;
using FluentScheduler;

namespace ApiMobaileTaxi.BackgroundService
{
    public class MyRegistr : Registry
    {
        public MyRegistr()
        {
            Schedule<InWorkDriver>().ToRunEvery(1).Days().At(6, 59);
            Schedule<OrderForDrivers>().ToRunNow().AndEvery(2).Minutes();
        }
    }
}