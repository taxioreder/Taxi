using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms.Xaml;

namespace TaxiApp.Service.GeloctionGPS
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class WarningModalPage : PopupPage
    {
		public WarningModalPage ()
		{
			InitializeComponent ();
		}

        [System.Obsolete]
        private async void Button_Clicked(object sender, System.EventArgs e)
        {
            await PopupNavigation.PopAsync();
            await Utils.StartListening(true);
        }
    }
}