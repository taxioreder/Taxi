using Plugin.Settings;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaxiApp.Models;
using TaxiApp.Service;
using TaxiApp.Service.Net;
using TaxiApp.View.A_R;
using Xamarin.Forms;

namespace TaxiApp.ViewModels.AppPageMV
{
    public class FullOrderMV : BindableBase
    {
        private ManagerTaxi managerTaxi = null;
        private INavigation Navigation { get; set; }
        public DelegateCommand RefreshCommand { get; set; }

        public FullOrderMV(ManagerTaxi managerTaxi, INavigation navigation)
        {
            Navigation = navigation;
            Orders = new List<Order>();
            this.managerTaxi = managerTaxi;
            RefreshCommand = new DelegateCommand(Init);
            Init();
        }

        internal void OutAccount()
        {
            CrossSettings.Current.Remove("Token");
            App.isAvtorization = false;
            App.Current.MainPage = new NavigationPage(new Avtorization());
        }

        private List<Order> orders = null;
        public List<Order> Orders
        {
            get => orders;
            set => SetProperty(ref orders, value);
        }

        private bool isRefr = false;
        public bool IsRefr
        {
            get => isRefr;
            set => SetProperty(ref isRefr, value);
        }

        [System.Obsolete]
        public async void Init()
        {
            IsRefr = true;
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            List<Order> orders1 = null;
            await Task.Run(() => Utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerTaxi.OrderWork("OrderGet", token, ref description, ref orders1);
                });
                if (state == 2)
                {
                    //await PopupNavigation.PushAsync(new Errror(description, null));
                }
                else if (state == 3)
                {
                    Orders = orders1;
                }
                else if (state == 4)
                {
                    //await PopupNavigation.PushAsync(new Errror("Technical work on the service", null));
                }
            }
            IsRefr = false;
        }

        private async void ActionForOrder()
        {
            if(Orders[0].CurrentOrder == "New")
            {
                //PopUp Info New Order Plese Accept
            }
            else if(Orders[0].CurrentOrder == "DriveFrome")
            {
                //PopUp Go to Navigation From Order, Yes No
            }
            else if (Orders[0].CurrentOrder == "DriveTo")
            {
                //PopUp Go to Navigation to Order, Yes No
            }
            else if (Orders[0].CurrentOrder == "NewNext")
            {
                //PopUp Info Order Avtomatik Accept Order of 5 min
            }
        }

        public async Task<location> GetLonAndLanToAddress(string address)
        {
            location location = null;
            await Task.Run(() => Utils.CheckNet());
            if (App.isNetwork)
            {
                bool isReqvest = managerTaxi.ApiGoogleMapsWork("GetGetLonAndLanToAddress", address, ref location);
                if (!isReqvest && location == null)
                {
                    location = null;
                    // poupTypeError
                }
            }
            return location;
        }
    }
}