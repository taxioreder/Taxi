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

        [System.Obsolete]
        public FullOrderMV(ManagerTaxi managerTaxi, INavigation navigation)
        {
            Navigation = navigation;
            this.managerTaxi = managerTaxi;
            RefreshCommand = new DelegateCommand(Init);
            App.initDasbordDelegate = Init;
            IsOrderMobile = true;
            Init();
        }

        internal void OutAccount()
        {
            CrossSettings.Current.Remove("Token");
            App.isAvtorization = false;
            App.Current.MainPage = new NavigationPage(new Avtorization());
        }

        private bool isEmty = false;
        public bool IsEmty
        {
            get => isEmty;
            set => SetProperty(ref isEmty, value);
        }

        private OrderMobile orderMobile = null;
        public OrderMobile OrderMobile
        {
            get => orderMobile;
            set => SetProperty(ref orderMobile, value);
        }

        private bool isRefr = false;
        public bool IsRefr
        {
            get => isRefr;
            set => SetProperty(ref isRefr, value);
        }

        private bool isOrderMobile = false;
        public bool IsOrderMobile
        {
            get => isOrderMobile;
            set => SetProperty(ref isOrderMobile, value);
        }

        [System.Obsolete]
        public async void Init()
        {
            IsRefr = true;
            string token = CrossSettings.Current.GetValueOrDefault("Token", "");
            string description = null;
            int state = 0;
            OrderMobile orderMobile1 = null;
            await Task.Run(() => Utils.CheckNet());
            if (App.isNetwork)
            {
                await Task.Run(() =>
                {
                    state = managerTaxi.OrderWork("OrderMobileGet", token, ref description, ref orderMobile1);
                });
                if (state == 2)
                {
                    //await PopupNavigation.PushAsync(new Errror(description, null));
                }
                else if (state == 3)
                {
                    OrderMobile = orderMobile1;
                }
                else if (state == 4)
                {
                    //await PopupNavigation.PushAsync(new Errror("Technical work on the service", null));
                }
            }
            IsOrderMobile = OrderMobile != null;
            IsEmty = OrderMobile == null;
            IsRefr = false;
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