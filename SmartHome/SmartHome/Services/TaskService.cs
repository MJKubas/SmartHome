﻿using Newtonsoft.Json;
using SmartHome.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        public async Task<MainDevice> GetInfoAsync(string deviceName, string address)
        {
            AuthRequest auth = new AuthRequest(deviceName);
            var jsonData = JsonConvert.SerializeObject(auth);
            //string jsonData = "{\"AuthToken\":\"TEST\"}";
            string uri = "http://" + address + ":8080/register";
            var client = new HttpClient();
            try
            {
                HttpResponseMessage response = await client.PostAsync(uri, new StringContent(jsonData, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                HttpContent content = response.Content;
                var result = await content.ReadAsStringAsync();
                MainDevice mainDevice = JsonConvert.DeserializeObject<MainDevice>(result);
                mainDevice.IpAddress = address;
                return mainDevice;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }
    }
}