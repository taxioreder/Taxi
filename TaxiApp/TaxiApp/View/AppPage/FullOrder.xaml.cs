using Plugin.Geofence;
using Plugin.Geofence.Abstractions;
using Plugin.Messaging;
using System;
using TaxiApp.Models;
using TaxiApp.Service;
using TaxiApp.ViewModels.AppPageMV;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TaxiApp.View.AppPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FullOrder : ContentPage
    {
        private FullOrderMV fullOrderMV = null;
            
        public FullOrder(ManagerTaxi managerTaxi)
        {
            InitializeComponent();
            fullOrderMV = new FullOrderMV(managerTaxi, Navigation);
            BindingContext = fullOrderMV;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            fullOrderMV.OutAccount();
        }

        private void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            var phoneDialer = CrossMessaging.Current.PhoneDialer;
            if (phoneDialer.CanMakePhoneCall)
            {
                phoneDialer.MakePhoneCall(((Label)sender).Text);
            }
        }

        private async void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
        {
            location locationFrom = await fullOrderMV.GetLonAndLanToAddress(fullOrderMV.Orders[0].FromAddress);
            location locationTo = await fullOrderMV.GetLonAndLanToAddress(fullOrderMV.Orders[0].ToAddress);
            if (locationFrom != null)
            {
                DependencyService.Get<Service.Geofence.IGeofence>().StartGeofence(fullOrderMV.Orders[0].ID.ToString(), Convert.ToDouble(locationFrom.lat), Convert.ToDouble(locationFrom.lng), Convert.ToDouble(locationTo.lat), Convert.ToDouble(locationTo.lng), 0.001000);
                var placemark = new Placemark
                {
                    Thoroughfare = fullOrderMV.Orders[0].FromAddress
                };
                var options = new MapLaunchOptions { Name = "1", NavigationMode = NavigationMode.Driving };
                await Map.OpenAsync(placemark, options);
            }
        }
    }
}