using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SmartHome.Services;
using SmartHome.Views;

namespace SmartHome
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
