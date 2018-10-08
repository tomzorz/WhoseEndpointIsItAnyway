using System;
using WhoseEndpointIsItAnyway;

namespace WhoseEndpointItIsAnyway.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var ep = NetworkHelper.GetFriendlyNeighborhoodEndpoint();
            Console.WriteLine($"friendly: {ep.Unicast} {ep.Broadcast} {ep.IsWan} {ep.Rank}");
            foreach (var rankEndpoint in NetworkHelper.RankEndpoints())
            {
                Console.WriteLine($"> {rankEndpoint.Rank} {rankEndpoint.Unicast} {rankEndpoint.Broadcast} {rankEndpoint.IsWan}");
            }
            Console.ReadKey();

            var ranked = NetworkHelper.RankEndpoints();
        }
    }
}
