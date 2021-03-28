using ShellProgressBar;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace GithubHostBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            
        }

        static string GetFastestIp(string[] ipList)
        {
            var speedOfIp = new Dictionary<string, long>();
            ProgressBar progressBar = new ProgressBar(ipList.Length, "Testing ip address");

            foreach (var item in ipList)
            {
                progressBar.Tick(item);
                List<long> speeds = new List<long>();
                Ping ping = new Ping();

                for (int i = 0; i < 4; i++)
                {
                    var reply = ping.Send(item, 300);
                    if (reply.Status == IPStatus.Success)
                        speeds.Add(reply.RoundtripTime);
                }

                if (speeds.Any())
                    speedOfIp.Add(item, (long) speeds.Average());
            }

            if (!speedOfIp.Any())
                throw new Exception("No available ip address");

            long fastestSpeed = speedOfIp.Min(i => i.Value);

            return speedOfIp.First(v => v.Value == fastestSpeed).Key;
        }

        static string[] GetIpList(string host)
        {
            Console.WriteLine($"Please enter a list of ip addresses of '{host}', separating each item with a comma");
            string ipAddresses = Console.ReadLine();
            return ipAddresses.Split(',');
        }
    }
}
