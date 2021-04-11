using SmartHome.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Mqtt;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SmartHome.Models.Sensors
{
    class CameraSensor : SensorDevice
    {
        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        private TaskService TaskService = new TaskService();

        public override void stopGetingValues()
        {
            tokenSource.Cancel();
        }
    }
}
