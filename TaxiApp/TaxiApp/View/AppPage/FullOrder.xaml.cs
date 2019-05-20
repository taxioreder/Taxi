using System;
using TaxiApp.Service;
using TaxiApp.ViewModels.AppPageMV;
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
    }
}