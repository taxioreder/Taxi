using Plugin.Settings;
using TaxiApp.Service.GeloctionGPS;
using TaxiApp.View.A_R;
using TaxiApp.View.AppPage;
using Xamarin.Forms;

namespace TaxiApp
{
    public partial class App : Application
    {
        internal static bool isAvtorization;
        internal static bool isNetwork;
        internal static bool isStart;

        public App()
        {
            InitializeComponent();
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            if (token == "")
            {
                isAvtorization = false;
                MainPage = new NavigationPage(new Avtorization());
            }
            else
            {
                isAvtorization = true;
                MainPage = new NavigationPage(new FullOrder(new Service.ManagerTaxi()));
            }
        }

        protected override async void OnStart()
        {
            if (isAvtorization)
            {
                isStart = true;
                Utils.StartListening();
            }
        }

        protected override async void OnSleep()
        {
            if (isAvtorization)
            {
                isStart = false;
                Utils.StopListening();
            }
        }

        protected override async void OnResume()
        {
            if (isAvtorization)
            {
                isStart = true;
                Utils.StartListening();
            }
        }
    }
}