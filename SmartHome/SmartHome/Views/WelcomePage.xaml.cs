using SmartHome.Models;
using SmartHome.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartHome.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WelcomePage : ContentPage
    {
        WelcomePageViewModel welcomePage;
        public WelcomePage()
        {
            InitializeComponent();
            welcomePage = new WelcomePageViewModel();
            //BindingContext = new WelcomePageViewModel();
            BindingContext = welcomePage;
        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var sensor = ((ListView)sender).SelectedItem as SensorDevice;
            if (sensor == null) return;
            await Navigation.PushAsync(new SensorDetailPage(sensor, welcomePage.GetMainDeviceAddress()));
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }
    }
}
