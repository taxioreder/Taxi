using Plugin.Settings;
using TaxiApp.View.A_R;
using Xamarin.Forms;

namespace TaxiApp
{
    public partial class App : Application
    {
        internal static bool isAvtorization;

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
                //MainPage = new NavigationPage(new TabPage(new Service.ManagerDispatchMob()));
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