using TaxiApp.ViewModels.Tabs;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace TaxiApp.View.Tabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabbedPage1 : Xamarin.Forms.TabbedPage
    {
        private TabMV tabMV = null;

        public TabbedPage1(TabMV tabMV)
        {
            InitializeComponent();
            this.tabMV = tabMV;
            NavigationPage.SetHasNavigationBar(this, false);
            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            BindingContext = this.tabMV;
        }
    }
}