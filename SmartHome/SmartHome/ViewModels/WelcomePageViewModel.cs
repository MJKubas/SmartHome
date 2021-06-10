using MvvmHelpers.Commands;
using SmartHome.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Mqtt;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SmartHome.ViewModels
{
    public class WelcomePageViewModel : BaseViewModel
    {
        MainDevice _mainDevice;
        public ICommand RefreshButton { get; set; }
        private readonly string MqttClientId = "androidApp";
        private CancellationTokenSource GetValueCancellation = new CancellationTokenSource();
        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        public ObservableCollection<SensorDevice> devicesList { get; set; }

        public WelcomePageViewModel()
        {
            Title = "Your Smart Home";
            devicesList = new ObservableCollection<SensorDevice>();
            GetMainDevice();
            
            RefreshButton = new MvvmHelpers.Commands.Command(Refresh);
        }

        private string _text = "Please wait for the connection...";

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        public void PrintText(string input) => Device.BeginInvokeOnMainThread(() => Text = $"{input}\n");

        private async Task AddDetectedDevices()
        {
            _mainDevice = await TaskService.GetInfoAsync(_mainDevice.DeviceName, _mainDevice.IpAddress);
            var mqttClient = await MqttClientAndSubscribe();

            await mqttClient.SubscribeAsync("alert", MqttQualityOfService.ExactlyOnce);
            StartCheckingAlert(mqttClient);

            for (int i = 0; i < _mainDevice.ConnectedDevices.Count; i++)
            {
                await mqttClient.SubscribeAsync(_mainDevice.ConnectedDevices[i].Topic, MqttQualityOfService.ExactlyOnce); //QoS2
                _mainDevice.ConnectedDevices[i].StartGettingValues(mqttClient);
                devicesList.Add(_mainDevice.ConnectedDevices[i]);
            }
        }

        public void StartCheckingAlert(IMqttClient mqttClient)
        {
            MqttMessage mqttMessage;
            Task.Factory.StartNew(async () =>
            {
                string result = "off";
                while (!tokenSource.Token.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    mqttClient
                        .MessageStream
                        .Where(msg => msg.Topic == "alert")
                        .Subscribe(msg => result = Encoding.Default.GetString(msg.Payload));

                    if (result == "on")
                    {
                        var answer = await App.Current.MainPage.DisplayAlert("ALERT!", "Something is wrong, disable alarm?", "Yes", "No");
                        if (answer)
                        {
                            var message = new MqttApplicationMessage("alert", Encoding.UTF8.GetBytes("off"));
                            await mqttClient.PublishAsync(message, MqttQualityOfService.ExactlyOnce); //QoS0
                        }
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }

        public void Refresh()
        {
            devicesList.Clear();
            tokenSource.Cancel();
            if(_mainDevice != null && _mainDevice.ConnectedDevices.Count != 0)
            {
                foreach (SensorDevice device in _mainDevice.ConnectedDevices)
                {
                    device.stopGetingValues();
                }
            }

            PrintText("Refreshing...");
            GetMainDevice();
        }

        public async Task GetMainDevice()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            string gate_ip = "192.168.0.104";//NetworkGateway();
            var tasks = new List<Task<MainDevice>>();
            string[] array = gate_ip.Split('.');
            for (int i = 2; i <= 255; i++)
            {
                string ping_var = array[0] + "." + array[1] + "." + array[2] + "." + i;
                tasks.Add(TaskService.SendGETAsync(ping_var, tokenSource.Token));
            }
            while (tasks.Count > 0)
            {
                var finishedTask = await Task.WhenAny(tasks);
                tasks.Remove(finishedTask);
                if (finishedTask.Result != null)
                {
                    _mainDevice = finishedTask.Result;
                    AddDetectedDevices();
                    PrintText($"Detected sensors:");
                    tokenSource.Cancel();
                    tokenSource.Dispose();
                    break;
                }
            }
        }


        private async Task<IMqttClient> MqttClientAndSubscribe()
        {
            try
            {
                var configuration = new MqttConfiguration();
                configuration.Port = 1235;
                var _mqttClient = await MqttClient.CreateAsync(_mainDevice.IpAddress, configuration);
                var _sessionState = await _mqttClient.ConnectAsync(new MqttClientCredentials(clientId: MqttClientId));

                return _mqttClient;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;

        }

        public string NetworkGateway()
        {
            var IpAddress = Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault();

            if (IpAddress != null)
            {
                return IpAddress.ToString();
            }
            return null;
        }

        public string GetMainDeviceAddress()
        {
            return _mainDevice.IpAddress;
        }

    }
}
