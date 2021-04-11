using Newtonsoft.Json;
using SmartHome.Models;
using SmartHome.Models.Sensors;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Net.Mqtt;

namespace SmartHome.Services
{
    public class TaskService
    {
        public async Task<MainDevice> SendGETAsync(string address, CancellationToken token)
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("http://" + address + ":8080");
            request.Method = HttpMethod.Get;
            request.Headers.Add("Accept", "application/json");
            var client = new HttpClient();
            try
            {
                Console.WriteLine(address + "GET");
                HttpResponseMessage response = await client.SendAsync(request, token);

                if (response.IsSuccessStatusCode)
                {
                    HttpContent content = response.Content;
                    var result = await content.ReadAsStringAsync();
                    if (result.Contains("DeviceName"))
                    {
                        MainDevice mainDevice = JsonConvert.DeserializeObject<MainDevice>(result);
                        mainDevice.IpAddress = address;
                        return mainDevice;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }

        public async Task<string> GetInfoAsync(string address, CancellationToken token)
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("http://" + address);
            request.Method = HttpMethod.Get;
            var client = new HttpClient();
            try
            {
                Console.WriteLine(address + "GET");
                HttpResponseMessage response = await client.SendAsync(request, token);

                if (response.IsSuccessStatusCode)
                {
                    HttpContent content = response.Content;
                    var result = await content.ReadAsStringAsync();
                    return result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return "";
        }
    }
}