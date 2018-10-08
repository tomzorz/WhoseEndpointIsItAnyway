using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace WhoseEndpointIsItAnyway
{
    /// <summary>
    /// Whose Endpoint Is It Anyway, the library where every result is helpful and the expectations against code without pop culture references do not matter
    /// </summary>
    public static class NetworkHelper
    {
        private static readonly string[] DerankingWords = { "HYPERV", "HYPER-V", "VIRTUALBOX", "NPCAP", "EMULATOR", "VETHERNET", "TEREDO", "HAMACHI" };

        /// <summary>
        /// Gets you the friendly neighborhood endpoint that you probably want to use in most scenarios
        /// </summary>
        /// <param name="includeWanIfAvailable">Choose the WAN address if the device has one instead of any other LAN address, defaults to false</param>
        /// <returns>The endpoint you probably want with the details you actually care about</returns>
        /// <remarks>The WAN address is only going to be returned if the device is not behind a router or anything else that'd get thet address. Also some ISPs NAT the users, so even when the device is directly connected "to the internet" it stil might not get a WAN address.</remarks>
        public static Endpoint GetFriendlyNeighborhoodEndpoint(bool includeWanIfAvailable = false)
        {
            var rankedList = RankEndpoints();
            return includeWanIfAvailable ? rankedList.First() : rankedList.First(x => !x.IsWan);
        }

        /// <summary>
        /// Ranks operational endpoints by "usability"
        /// </summary>
        /// <returns>A list of ranked endpoints with the details you actually care about</returns>
        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        public static List<Endpoint> RankEndpoints()
        {
            // local method for splitting ip into int parts
            byte[] CreateIpParts(string ipv4Address)
            {
                try
                {
                    return ipv4Address.Split(new[] {"."}, StringSplitOptions.RemoveEmptyEntries).Select(byte.Parse).ToArray();
                }
                catch
                {
                    return new byte[] {0, 0, 0, 0};
                }
                
            }

            // local method for ranking an ip
            int CreateRankFromIpv4(byte[] parts)
            {
                // error
                if (parts.All(x => x == 0)) return 6;
                // probably the most common range for consumer grade routers
                if (parts[0] == 192 && parts[1] == 168) return 1;
                // 2nd most common range, or most if we go by enterprises
                if (parts[0] == 10) return 2;
                // highly unusual, probably vLans, virtual machine networking and others...
                if (parts[0] == 172 && (parts[1] >= 16 && parts[1] <= 31)) return 3;
                // link local addresses https://tools.ietf.org/html/rfc3927
                if (parts[0] == 169 && parts[1] == 254) return 5;
                // if none of the above it has to be a WAN address unless someone seriously misconfigured something
                // according to RFC1918 http://www.faqs.org/rfcs/rfc1918.html
                return 0;
            }

            // get all network interfaces
            return NetworkInterface.GetAllNetworkInterfaces()
                // get the IP properties of these
                .Select(x => (networkInterface: x, properties: x.GetIPProperties()))
                // filter to the ones that are operational, not loopback (as that's a given) and have IPv4 addresses
                .Where(y => y.networkInterface.OperationalStatus == OperationalStatus.Up 
                            && y.networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback
                            && y.properties.UnicastAddresses.Any(z => z.Address.AddressFamily == AddressFamily.InterNetwork))
                // get the data we need plus the ranks
                .Select(m =>
                {
                    // get ipv4 address
                    var ipv4 = m.properties.UnicastAddresses.First(n => n.Address.AddressFamily == AddressFamily.InterNetwork).Address.ToString();
                    // split it into pieces
                    var parts = CreateIpParts(ipv4);
                    // create rank
                    var rank = CreateRankFromIpv4(parts);
                    var nupper = m.networkInterface.Name.ToUpperInvariant();
                    if (DerankingWords.Any(x => nupper.Contains(x))) rank = 4;
                    // get subnet mask
                    var mask = m.properties.UnicastAddresses.First(n => n.Address.AddressFamily == AddressFamily.InterNetwork).IPv4Mask.ToString();
                    var maskParts = CreateIpParts(mask);
                    // calculate broadcast address
                    var broadcast = string.Join(".", parts.Zip(maskParts, (addr, maddr) => ((byte)(addr | ~maddr)).ToString()));
                    return new Endpoint(ipv4, rank == 0, broadcast, rank);
                })
                // order and return them
                .OrderBy(x => x.Rank).ToList();
        }

    }
}
