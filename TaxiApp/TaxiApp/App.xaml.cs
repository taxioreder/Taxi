using Plugin.Settings;
using TaxiApp.View.A_R;
using TaxiApp.View.AppPage;
using Xamarin.Forms;

namespace TaxiApp
{
    public partial class App : Application
    {
        internal static bool isAvtorization;
        internal static bool isNetwork = true;

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

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}