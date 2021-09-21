using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace Hardware.Windows.Network
{
    public static class NetworkInfo
    {
        private static int InterfaceIndex;
        private static string ID;

        public static string InterfaceID
        {
            get { return ID; }
            set
            {
                GetInterfaceIndex(value);
                ID = value;
            }
        }
        public static long ByteReceived { get; set; }
        public static long ByteSend { get; set; }

        public static List<Adapter> GetInterface()
        {
            List<Adapter> adapters = new List<Adapter>();

            NetworkInterface[] networks = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface net in networks)
            {
                if (net.Speed != 0)
                {
                    Adapter adapter = new Adapter();
                    adapter.Name = net.Name;
                    adapter.ID = net.Id;

                    adapters.Add(adapter);
                }
            }

            return adapters;
        }

        private static void GetInterfaceIndex(string id)
        {
            if (id == null) return;

            NetworkInterface[] networks = NetworkInterface.GetAllNetworkInterfaces();

            for (byte b = 0; b < networks.Length; b++)
            {
                if (networks[b].Id.Contains(id))
                {
                    InterfaceIndex = b;
                    break;
                }
            }
        }

        public static void SpeedUpdate()
        {
            IPv4InterfaceStatistics statistics = NetworkInterface.GetAllNetworkInterfaces()[InterfaceIndex].GetIPv4Statistics();

            ByteReceived = statistics.BytesReceived;
            ByteSend = statistics.BytesSent;
        }
    }
}
