using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SmartHome.Models;
using SmartHome.Views;
using SmartHome.ViewModels;

namespace SmartHome.Views
{
    [DesignTimeVisible(false)]
    public partial class SensorDetailPage : ContentPage
    {
        SensorDetailViewModel viewModel;

        public SensorDetailPage(SensorDevice sensor, string mainDeviceAddress)
        {
            InitializeComponent();

            BindingContext = viewModel = new SensorDetailViewModel(sensor, mainDeviceAddress);
        }

        //async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        //{
        //    var item = args.SelectedItem as Item;
        //    if (item == null)
        //        return;

        //    await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));

        //    // Manually deselect item.
        //    ItemsListView.SelectedItem = null;
        //}

        //async void AddItem_Clicked(object sender, EventArgs e)
        //{
        //    await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
        //}

        //protected override void OnAppearing()
        //{
        //    base.OnAppearing();

        //    if (viewModel.Items.Count == 0)
        //        viewModel.LoadItemsCommand.Execute(null);
        //}
    }
}