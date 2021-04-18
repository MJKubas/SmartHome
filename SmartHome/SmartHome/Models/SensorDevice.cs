using System;
using System.Collections.Generic;
using System.Net.Mqtt;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SmartHome.Models
{
    public class SensorDevice
    {
        public string Type { get; set; }
        public string Topic { get; set; }
        public string View { get; set; }
        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        Label sensorValue = new Label() { Text = "Initializing", FontSize = 15, BackgroundColor=Color.FromHex("#56c465"), Padding = new Thickness(10, 5, 10, 10) };
        Frame sensorTile = new Frame() { CornerRadius = 30, BorderColor = Color.Black, Padding = 0 };

        public virtual void stopGetingValues()
        {
            tokenSource.Cancel();
        }

        public virtual View CreateViewMqtt(IMqttClient mqttClient)
        {
            StackLayout stackLayout = new StackLayout() { BackgroundColor = Color.FromHex("#56c465"), Padding = new Thickness(10,5,10,5) };
            
            Label sensorLabel = new Label() { Text = "This is " + Type + " " + Topic, FontSize = 15, BackgroundColor = Color.FromHex("#56c465"), Padding = new Thickness(10,10,10,5)};

            Task.Factory.StartNew(async () =>
            {
                string result = "No data gathered yet";
                while (!tokenSource.Token.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    mqttClient
                        .MessageStream
                        .Where(msg => msg.Topic == Topic)
                        .Subscribe(msg => result = Encoding.Default.GetString(msg.Payload));

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        sensorValue.Text = result;
                    });
                }
            }, TaskCreationOptions.LongRunning);

            stackLayout.Children.Add(sensorLabel);
            stackLayout.Children.Add(sensorValue);
            sensorTile.Content = stackLayout;
            return sensorTile;
        }
    }
}
