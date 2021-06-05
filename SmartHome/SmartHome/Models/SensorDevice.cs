using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Mqtt;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SmartHome.Models
{
    public class SensorDevice : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _type = String.Empty;
        private string _topic = String.Empty;
        private string _view = String.Empty;
        private ImageSource _image = String.Empty;
        private string _value = String.Empty;

        public string Type { get { return this._type; } set { _type = value; NotifyPropertyChanged(); } }
        public string Topic { get { return this._topic; } set { _topic = value; NotifyPropertyChanged(); } }
        public string View { get { return this._view; } set { _view = value; NotifyPropertyChanged(); } }
        public string Value
        {
            get { return this._value; }
            set
            {
                if (value != _value)
                {
                    _value = value;
                    if (View == "number")
                    {
                        int parsedValue = -1000;
                        bool ifParsedValue = Int32.TryParse(value, out parsedValue);
                        if (Type == "TempSens")
                        {
                            switch (parsedValue)
                            {
                                case var _ when (parsedValue <= 5 && parsedValue > -1000):
                                    this.Image = ImageSource.FromResource("SmartHome.Assets.tempCold.png");
                                    break;
                                case var _ when (parsedValue < 20 & parsedValue > 5):
                                    this.Image = ImageSource.FromResource("SmartHome.Assets.tempWarm.png");
                                    break;
                                case var _ when (parsedValue >= 20):
                                    this.Image = ImageSource.FromResource("SmartHome.Assets.tempHot.png");
                                    break;
                                default:
                                    this.Image = ImageSource.FromResource("SmartHome.Assets.tempDefault.png");
                                    break;
                            }
                            
                        }
                        else if (Type == "HumSens")
                        {
                            switch (parsedValue)
                            {
                                case var _ when (parsedValue <= 30 && parsedValue > -1000):
                                    this.Image = ImageSource.FromResource("SmartHome.Assets.humLow.png");
                                    break;
                                case var _ when (parsedValue < 80 && parsedValue > 30):
                                    this.Image = ImageSource.FromResource("SmartHome.Assets.humMid.png");
                                    break;
                                case var _ when (parsedValue >= 80):
                                    this.Image = ImageSource.FromResource("SmartHome.Assets.humHigh.png");
                                    break;
                                default:
                                    this.Image = ImageSource.FromResource("SmartHome.Assets.humDefault.png");
                                    break;
                            }
                        }
                    }
                    else if (View == "text")
                    {
                        if (Type == "SoundSens")
                        {
                            switch (value)
                            {
                                case "cicho":
                                    this.Image = ImageSource.FromResource("SmartHome.Assets.soundLow.png");
                                    break;
                                case "neutralnie":
                                    this.Image = ImageSource.FromResource("SmartHome.Assets.soundMid.png");
                                    break;
                                case "glosno":
                                    this.Image = ImageSource.FromResource("SmartHome.Assets.soundHigh.png");
                                    break;
                                default:
                                    this.Image = ImageSource.FromResource("SmartHome.Assets.soundDefault.png");
                                    break;
                            }
                        }
                    }
                    else if (View == "switch")
                    {

                    }
                    else
                    {
                        this.Image = ImageSource.FromResource("SmartHome.Assets.default.png");
                    }
                    NotifyPropertyChanged();
                }
            }
        }

        public ImageSource Image { get { return this._image; } set { _image = value; NotifyPropertyChanged(); } }
        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        public virtual void stopGetingValues()
        {
            tokenSource.Cancel();
        }

        public void StartGettingValues(IMqttClient mqttClient)
        {
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
                        Value = result;
                    });
                }
            }, TaskCreationOptions.LongRunning);
        }
    }
}
