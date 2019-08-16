using System.Threading.Tasks;
using Plugin.Settings;
using Prism.Commands;
using Prism.Mvvm;
using Rg.Plugins.Popup.Services;
using TaxiApp.Service;
using TaxiApp.Service.GeloctionGPS;
using TaxiApp.StoreNotify;
using TaxiApp.View;
using TaxiApp.View.AppPage;
using Xamarin.Forms;

namespace TaxiApp.ViewModels.A_RVM
{
    class AvtorizationMV : BindableBase
    {
        private ManagerTaxi managerTaxi = null;
        public DelegateCommand AvtorizationCommand { get; set; }

        public AvtorizationMV()
        {
            managerTaxi = new ManagerTaxi();
            AvtorizationCommand = new DelegateCommand(Avtorization);
        }

        private string username;
        public string Username
        {
            get => username;
            set => SetProperty(ref username, value);
        }

        private string password;
        public string Password
        {
            get => password; 
            set => SetProperty(ref password, value);
        }

        private string feedBack;
        public string FeedBack
        {
            get => feedBack;
            set => SetProperty(ref feedBack, value);
        }

        private async void Avtorization()
        {
            await PopupNavigation.PushAsync(new LoadPage(), true);
            string token = null;
            string description = null;
            int state = 3;
            await Task.Run(() =>
            {
                state = managerTaxi.A_RWork("authorisation", Username, Password, ref description, ref token);
            });
            await PopupNavigation.PopAsync(true);
            if (state == 1)
            {
                //await PopupNavigation.PushAsync(new Errror(FeedBack, null));
                FeedBack = "Not Network";
            }
            else if (state == 2)
            {
                //await PopupNavigation.PushAsync(new Errror(FeedBack, null));
                FeedBack = description;
            }
            else if (state == 3)
            {
                App.isAvtorization = true;
                CrossSettings.Current.AddOrUpdateValue("Token", token);
                await Task.Run( async() =>
                {
                    await Utils.StartListening();
                });
                App.Current.MainPage = new NavigationPage(new FullOrder(managerTaxi));
                DependencyService.Get<IStore>().OnTokenRefresh();
            }
            else if (state == 4)
            {
                //await PopupNavigation.PushAsync(new Errror(FeedBack, null));
                FeedBack = "Technical work on the service";
            }
        }
    }
}