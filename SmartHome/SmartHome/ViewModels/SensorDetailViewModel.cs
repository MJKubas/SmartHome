using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using SmartHome.Models;
using SmartHome.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Microcharts;
using SkiaSharp;

namespace SmartHome.ViewModels
{
    public class SensorDetailViewModel : BaseViewModel, INotifyPropertyChanged
    {
        public new event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private List<DataItem> _sensorData;
        private ChartEntry[] _chartEntries;
        private List<ChartEntry> entryList = new List<ChartEntry>();
        private LineChart _lineChart;

        public SensorDevice Sensor { get; set; }
        public string _mainDeviceAddress { get; set; }
        public List<DataItem> SensorData { get { return _sensorData; } set { _sensorData = value; NotifyPropertyChanged(); } }
        public ChartEntry[] ChartEntries { get { return _chartEntries; } set { _chartEntries = value; NotifyPropertyChanged(); } }
        public LineChart LineChart { get { return _lineChart; } set { _lineChart = value; NotifyPropertyChanged(); } }

        public SensorDetailViewModel(SensorDevice sensor, string mainDeviceAddress)
        {
            Title = "Details";
            Sensor = sensor;
            _mainDeviceAddress = mainDeviceAddress;
            LoadData();
        }

        private async Task LoadData()
        {
            SensorData = await Services.TaskService.GetData(_mainDeviceAddress, Sensor.Topic);
            foreach (var element in SensorData)
            {
                float value;
                string avg = element.avgValue;

                int index = avg.IndexOf(".");
                if (index > 0)
                    avg = avg.Substring(0, index);

                if (float.TryParse(avg, out value))
                {
                    var parsedDate = DateTime.Parse(element.day);
                    ChartEntry chartEntry = new ChartEntry(value)
                    {
                        Label = parsedDate.ToString("dd-MM-yyyy"),
                        ValueLabel = avg,
                        Color = SKColor.Parse("#56c465")
                    };
                    entryList.Add(chartEntry);
                }
            }
            ChartEntries = entryList.ToArray();
            LineChart chart = new LineChart() {
                Entries = ChartEntries,
                LabelTextSize = 30,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelOrientation = Orientation.Horizontal,
                ValueLabelTextSize = 30
            };
            LineChart = chart;
        }
    }
}