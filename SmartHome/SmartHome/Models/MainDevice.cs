using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHome.Models
{
    public class MainDevice
    {
        public string IpAddress { get; set; }
        public string DeviceName { get; set; }
        public List<SensorDevice> ConnectedDevices { get; set; }
    }
}
