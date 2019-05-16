namespace ApiMobaileTaxi.Service
{
    public class ManagerTaxiApi
    {
        SqlCoommandTaxiApi sqlCoommandTaxiApi = null;

        public ManagerTaxiApi()
        {
            sqlCoommandTaxiApi = new SqlCoommandTaxiApi();
        }
    }
}