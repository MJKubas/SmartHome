using MvvmHelpers.Commands;
using SmartHome.Models;
using SmartHome.Models.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mqtt;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SmartHome.ViewModels
{
    public class WelcomePageViewModel : BaseViewModel
    {
        Grid _grid;
        MainDevice _mainDevice;
        public ICommand RefreshButton { get; set; }
        private readonly string MqttClientId = "androidApp";
        private CancellationTokenSource GetValueCancellation = new CancellationTokenSource();
        public WelcomePageViewModel(Grid grid)
        {
            Title = "Your Smart Home";
            _grid = grid;
            GetMainDevice();
            
            RefreshButton = new MvvmHelpers.Commands.Command(Refresh);
        }

        private string _text = "Please wait for the connection with Your main device";

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        public void PrintText(string input) => Device.BeginInvokeOnMainThread(() => Text = $"{input}\n");

        private async Task AddDetectedDevices()
        {
            List<SensorDevice> sensorDevices = new List<SensorDevice>();

            var mqttClient = await MqttClientAndSubscribe();

            for (int i = 0; i < _mainDevice.ConnectedDevices.Count; i++)
            {
                switch (_mainDevice.ConnectedDevices[i].Type)
                {
                    case "CameraSensor":
                        CameraSensor cameraSensor = new CameraSensor { Topic = _mainDevice.ConnectedDevices[i].Topic };
                        sensorDevices.Add(cameraSensor);
                        break;
                    case "HumiditySensor":
                        HumiditySensor humiditySensor = new HumiditySensor { Topic = _mainDevice.ConnectedDevices[i].Topic };
                        sensorDevices.Add(humiditySensor);
                        break;
                    case "MotionSensor":
                        MotionSensor motionSensor = new MotionSensor { Topic = _mainDevice.ConnectedDevices[i].Topic };
                        sensorDevices.Add(motionSensor);
                        break;
                    case "RfidSensor":
                        RfidSensor rfidSensor = new RfidSensor { Topic = _mainDevice.ConnectedDevices[i].Topic };
                        sensorDevices.Add(rfidSensor);
                        break;
                    case "SoundSensor":
                        SoundSensor soundSensor = new SoundSensor { Topic = _mainDevice.ConnectedDevices[i].Topic };
                        sensorDevices.Add(soundSensor);
                        break;
                    case "TemperatureSensor":
                        TemperatureSensor temperatureSensor = new TemperatureSensor { Topic = _mainDevice.ConnectedDevices[i].Topic };
                        sensorDevices.Add(temperatureSensor);
                        break;
                    default:
                        SensorDevice unknownSensor = new SensorDevice { Topic = _mainDevice.ConnectedDevices[i].Topic, Type = _mainDevice.ConnectedDevices[i].Type };
                        sensorDevices.Add(unknownSensor);
                        break;
                };

                await mqttClient.SubscribeAsync(_mainDevice.ConnectedDevices[i].Topic, MqttQualityOfService.ExactlyOnce); //QoS2
                _grid.Children.Add(sensorDevices[i].CreateViewMqtt(mqttClient), 0, i);
            }
            _mainDevice.ConnectedDevices = sensorDevices;
        }

        public void Refresh()
        {
            _grid.Children.Clear();
            if(_mainDevice.ConnectedDevices.Count != 0)
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

    }
}
