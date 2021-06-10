using System;
using System.Collections.Generic;
using System.Text;

namespace SmartHome.Models
{
    class MqttMessage
    {
        public SensorDummy Device { get; set; }
        public string AuthToken { get; set; }
        public string MqttType { get; set; }
        public string Value { get; set; }
    }
}
