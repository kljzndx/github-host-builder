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
            string[] allHosts = 
            {
                "github.com",
                "gist.github.com",
                "assets-cdn.github.com",
                "raw.githubusercontent.com",
                "cloud.githubusercontent.com",
                "camo.githubusercontent.com",
                "avatars0.githubusercontent.com",
                "avatars1.githubusercontent.com",
                "avatars2.githubusercontent.com",
                "avatars3.githubusercontent.com",
                "avatars4.githubusercontent.com",
                "avatars5.githubusercontent.com",
                "avatars6.githubusercontent.com",
                "avatars7.githubusercontent.com",
                "avatars8.githubusercontent.com"
            };

            string[] needCheckHosts = allHosts.Take(4).ToArray();
            var ipListOfHost = new Dictionary<string, string[]>();

            foreach (var item in needCheckHosts)
                ipListOfHost.Add(item, GetIpList(item));

            var bastIpList = new List<string>();

            foreach (var item in ipListOfHost)
            {
                try
                {
                    bastIpList.Add(GetFastestIp(item.Key, item.Value));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            }

            Console.WriteLine("Begin building");

            var hostsFile = new List<string>();
            for (int i = 0; i < allHosts.Length; i++)
            {
                string bastIp = i < bastIpList.Count ? bastIpList[i] : bastIpList.Last();
                hostsFile.Add($"{bastIp}    {allHosts[i]}");
            }

            Console.WriteLine("Complete build");
            Console.WriteLine();
            foreach (var item in hostsFile)
                Console.WriteLine(item);
        }

        static string GetFastestIp(string host, string[] ipList)
        {
            Console.WriteLine($"Testing ip address list of '{host}'");
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
                    speedOfIp.Add(item, (long)speeds.Average());
            }

            progressBar.Dispose();
            Console.WriteLine();

            if (!speedOfIp.Any())
                throw new Exception("No available ip address");

            long fastestSpeed = speedOfIp.Min(i => i.Value);
            var fastestIp = speedOfIp.First(v => v.Value == fastestSpeed);

            Console.WriteLine(fastestIp.Key + "    " + fastestIp.Value + "ms");
            return fastestIp.Key;
        }

        static string[] GetIpList(string host)
        {
            Console.WriteLine($"Please enter a list of ip addresses of '{host}', separating each item with a comma");
            string ipAddresses = Console.ReadLine();
            return ipAddresses.Split(',');
        }
    }
}
