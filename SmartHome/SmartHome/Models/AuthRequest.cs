using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartHome.Models
{
    class AuthRequest
    {
        public AuthRequest(string deviceName)
        {
            DeviceName = deviceName;
            AuthToken = GenerateToken();
        }
        public string AuthToken;
        public string MqttType;
        private string DeviceName;

        public string GenerateToken()
        {
            donexzcx
            //var now = DateTime.Now.ToString("MM/dd/yyyy hh:mm");
            //string binaryNow = ToBinary(ConvertToByteArray(now, Encoding.ASCII));
            //string binaryDeviceName = ToBinary(ConvertToByteArray(DeviceName, Encoding.ASCII));
            //int binaryNowInt = Convert.ToInt32(binaryNow, 2);
            //int binaryDeviceNameInt = Convert.ToInt32(binaryDeviceName, 2);
            //int seed = binaryDeviceNameInt + binaryNowInt;
            return "TEST";
        }

        public static byte[] ConvertToByteArray(string str, Encoding encoding)
        {
            return encoding.GetBytes(str);
        }

        public static String ToBinary(Byte[] data)
        {
            return string.Join(" ", data.Select(byt => Convert.ToString(byt, 2).PadLeft(8, '0')));
        }
    }
}
