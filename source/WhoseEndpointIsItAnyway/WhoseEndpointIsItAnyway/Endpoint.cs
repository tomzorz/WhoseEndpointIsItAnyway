namespace WhoseEndpointIsItAnyway
{
    /// <summary>
    /// An endpoint descriptor with the details you actually care about
    /// </summary>
    public class Endpoint
    {
        /// <summary>
        /// The regular IPv4 address of this device on the network this endpoint connects to
        /// </summary>
        public string Unicast { get; }

        /// <summary>
        /// Signals if the unicast address is a WAN address
        /// </summary>
        public bool IsWan { get; }

        /// <summary>
        /// The broadcast address for the subnet this endpoint is connecting to (this is what you'll want to use for e.g. network discovery)
        /// </summary>
        public string Broadcast { get; }

        /// <summary>
        /// The rank of this address, as follows: 0 - WAN; 1 & 2 - Probably what you want; 3 - Might be what you want, but probably infrastructure for virtual machines or something similar; 4 - most definitely infrastructure for virtual machines or something similar; 5 - link local addresses; 6 - something wrong, don't use it
        /// </summary>
        public int Rank { get; }

        internal Endpoint(string unicast, bool isWan, string broadcast, int rank)
        {
            Unicast = unicast;
            IsWan = isWan;
            Broadcast = broadcast;
            Rank = rank;
        }
    }
}
