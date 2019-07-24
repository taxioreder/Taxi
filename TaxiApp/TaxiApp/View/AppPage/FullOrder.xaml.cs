using Plugin.Messaging;
using System;
using System.Threading;
using TaxiApp.Models;
using TaxiApp.Service;
using TaxiApp.Service.Geofence;
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
        private Timer timer = null;

        public FullOrder(ManagerTaxi managerTaxi)
        {
            InitializeComponent();
            fullOrderMV = new FullOrderMV(managerTaxi, Navigation);
            BindingContext = fullOrderMV;
        }

        private async void ReminderTrackInspaction(object s)
        {
            if (fullOrderMV.OrderMobile != null && fullOrderMV.OrderMobile.OnePointForAddressOrders[0].Status == null)
            {
                await blockOrder.RotateTo(3, 50);
                Vibration.Vibrate(30);
                await blockOrder.RotateTo(-3, 50);
                Vibration.Vibrate(30);
                await blockOrder.RotateTo(3, 50);
                Vibration.Vibrate(30);
                await blockOrder.RotateTo(-3, 50);
                Vibration.Vibrate(30);
                await blockOrder.RotateTo(3, 50);
                Vibration.Vibrate(30);
                await blockOrder.RotateTo(-3, 50);
                Vibration.Vibrate(30);
                await blockOrder.RotateTo(3, 50);
                Vibration.Vibrate(30);
                await blockOrder.RotateTo(-3, 50);
                Vibration.Vibrate(30);
                await blockOrder.RotateTo(0, 50);
                Vibration.Vibrate(30);
            }
            else
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
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

        private void TapGestureRecognizer_Tapped_6(object sender, EventArgs e)
        {
            StackLayout stackLayout = ((Label)sender).FindByName<StackLayout>("infoB");
            if(stackLayout.IsVisible)
            {
                stackLayout.IsVisible = false;
            }
            else
            {
                stackLayout.IsVisible = true;
            }
        }

        private void TapGestureRecognizer_Tapped_7(object sender, EventArgs e)
        {
            StackLayout stackLayout = ((Label)sender).FindByName<StackLayout>("stepB");
            if (stackLayout.IsVisible)
            {
                stackLayout.IsVisible = false;
            }
            else
            {
                stackLayout.IsVisible = true;
            }
        }

        [Obsolete]
        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (fullOrderMV.OrderMobile != null)
            {
                fullOrderMV.IsRefr = true;
                GefenceManager gefenceManager = new GefenceManager();
                DependencyService.Get<Service.Geofence.IGeofence>().StartGeofence(fullOrderMV.OrderMobile);
                var options = new MapLaunchOptions { Name = "1", NavigationMode = Xamarin.Essentials.NavigationMode.Driving };
                await Map.OpenAsync(fullOrderMV.OrderMobile.OnePointForAddressOrders[0].Lat, fullOrderMV.OrderMobile.OnePointForAddressOrders[0].Lng, options);
                await gefenceManager.RecurentStatusOrder(fullOrderMV.OrderMobile.ID, "StartOrder");
                fullOrderMV.Init();
            }
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            fullOrderMV.IsRefr = true;
            DependencyService.Get<Service.Geofence.IGeofence>().ContinueGeofence();
            fullOrderMV.Init();
        }

        private void Button_Clicked_2(object sender, EventArgs e)
        {
            fullOrderMV.IsRefr = true;
            DependencyService.Get<Service.Geofence.IGeofence>().EndGeofence();
            fullOrderMV.Init();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            timer = new Timer(new TimerCallback(ReminderTrackInspaction), null, 10000, 10000);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private async void Button_Clicked_3(object sender, EventArgs e)
        {
            Label label = ((Label)sender).FindByName<Label>("addressL");
        }

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            OnePointForAddressOrder onePointForAddressOrder = (OnePointForAddressOrder)e.Item;
            if (onePointForAddressOrder.Status == "DriveFromPoint")
            {
                var options = new MapLaunchOptions { Name = "1", NavigationMode = Xamarin.Essentials.NavigationMode.Driving };
                await Map.OpenAsync(onePointForAddressOrder.Lat, onePointForAddressOrder.Lng, options);
            }
        }

        [Obsolete]
        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            fullOrderMV.Init();
        }

        private void StackLayout_SizeChanged(object sender, EventArgs e)
        {
            StackLayout stackLayout = (StackLayout)sender;
            stackLayout.HeightRequest = App.Current.MainPage.Height * 0.98;
            stackLayout.WidthRequest = App.Current.MainPage.Width * 0.98;
        }
    }
}