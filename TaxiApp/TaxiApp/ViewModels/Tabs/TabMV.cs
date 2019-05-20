using TaxiApp.Service;

namespace TaxiApp.ViewModels.Tabs
{
    public class TabMV
    {
        private ManagerTaxi managerTaxi = null;

        public TabMV(ManagerTaxi managerTaxi)
        {
            this.managerTaxi = managerTaxi;
        }
    }
}