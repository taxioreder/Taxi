using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiApp.ViewModels.A_RVM;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TaxiApp.View.A_R
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Avtorization : ContentPage
    {
        private AvtorizationMV avtorizationMV = null;

        public Avtorization()
        {
            InitializeComponent();
            avtorizationMV = new AvtorizationMV();
            NavigationPage.SetHasNavigationBar(this, false);
            BindingContext = avtorizationMV;
        }
    }
}