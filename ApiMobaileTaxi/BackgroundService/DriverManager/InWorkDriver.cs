using ApiMobaileTaxi.Service;
using FluentScheduler;

namespace ApiMobaileTaxi.BackgroundService.DriverManager
{
    public class InWorkDriver : IJob
    {
        SqlCoommandTaxiApi sqlCoommandTaxiApi = null;

        public void Execute()
        {
            sqlCoommandTaxiApi = new SqlCoommandTaxiApi();
            sqlCoommandTaxiApi.SetWorkDrive();
        }
    }
}
