# WhoseEndpointIsItAnyway

*Whose Endpoint Is It Anyway, the library where every result is helpful and the expectations against code without pop culture references do not matter* mhm.. **cough...** what I meant to say was:

A tiny .net standard library to help you find your friendly neighborhood endpoint in the sea of vEthernets and npcap loopback points.

![](https://img.shields.io/badge/platform-any-green.svg?longCache=true&style=flat-square) ![](https://img.shields.io/badge/nuget-yes-green.svg?longCache=true&style=flat-square) ![](https://img.shields.io/badge/license-MIT-blue.svg?longCache=true&style=flat-square)

## Usage

For most usecases you only need 2 lines.

```csharp
using WhoseEndpointIsItAnyway;
// ...
var ep = NetworkHelper.GetFriendlyNeighborhoodEndpoint();

Console.WriteLine($"What you're looking for is: {ep.Unicast} {ep.Broadcast} {ep.IsWan}");

```

You can also pass a `true` to the method above to return the WAN address if the device has one.

## Ranking system

You can also ask for a ranked (and sorted) list of all the operational endpoints.

```csharp
var ranked = NetworkHelper.RankEndpoints();
```

Every returned endpoint has a `Rank` property that can range from 0 to 6 as follows:

0. WAN
1. probably what you want (usually home, small office)
2. probably what you want (usually enterprise)
3. might be what you want, but probably infrastructure for virtual machines or something similar
4. most definitely infrastructure for virtual machines or something similar
5. link local addresses
6. something wrong, don't use it