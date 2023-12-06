using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

internal class Program
{
    private static readonly Regex SeedsRegex = new Regex(@"^seeds:(?:\s(?<seed>\d+))+$");
    private static readonly Regex MapNameRegex = new Regex(@"^(?<map>.+) map:");
    private static readonly Regex MapRangesRegex = new Regex(@"^(?<destination>\d+)\s+(?<source>\d+)\s+(?<range>\d+)$");

    private static void Main(string[] args)
    {
        var input = ParseInput();
        Console.WriteLine(Solve2(input));
    }

    private static IEnumerable<long> ComputeHops(Map map, long source)
    {
        var found = false;
        foreach (var rangeDescriptor in map.Ranges)
        {
            if (
                source >= rangeDescriptor.Source &&
                source <= rangeDescriptor.Source + rangeDescriptor.Range
            )
            {
                found = true;
                var delta = source - rangeDescriptor.Source;
                yield return rangeDescriptor.Destination + delta;
            }
        }
        if (!found)
        {
            yield return source;
        }
    }

    private static long ComputeMinLocationEnd(Almanac almanac, long source, long mapNumber)
    {
        if (mapNumber == almanac.Maps.Length - 1)
        {
            return ComputeHops(almanac.Maps[mapNumber], source).Min();
        }
        var minValue = long.MaxValue;
        long currentValue;
        foreach (var hop in ComputeHops(almanac.Maps[mapNumber], source))
        {
            currentValue = ComputeMinLocationEnd(almanac, hop, mapNumber + 1);
            if (currentValue < minValue)
            {
                minValue = currentValue;
            }
        }
        return minValue;
    }

    private static long Solve1(Almanac almanac) =>
        almanac
            .Seeds
            .Select(
                seed =>
                    ComputeMinLocationEnd(almanac, seed, 0))
            .Min()
    ;

    private static long Solve2(Almanac almanac) =>
        ComputeRanges(almanac)
            .AsParallel()
            .Distinct()
            .Select(seed => ComputeMinLocationEnd(almanac, seed, 0))
            .Min()
    ;

    private static IEnumerable<long> ComputeRanges(Almanac almanac)
    {
        for (var i = 0; i < almanac.Seeds.Length; i += 2)
        {
            for (var increase = 0l; increase < almanac.Seeds[i + 1]; increase++)
            {
                yield return almanac.Seeds[i] + increase;
            }
        }
    }

    private static Almanac ParseInput()
    {
        string s, currentMapName;
        var seeds = new List<long>();
        var maps = new List<Map>();
        // parse seeds
        s = Console.ReadLine();
        var seedsMatch = SeedsRegex.Match(s);
        foreach (Capture c in seedsMatch.Groups["seed"].Captures)
        {
            seeds.Add(long.Parse(c.Value));
        }
        // new line
        Console.ReadLine();
        // maps
        while (true)
        {
            // map name
            s = Console.ReadLine();
            currentMapName = MapNameRegex.Match(s).Groups["map"].Value;
            var ranges = new List<RangeDescriptor>();
            while (true)
            {
                s = Console.ReadLine();
                if (s is null || s.Length == 0)
                {
                    break;
                }
                var rangeMatch = MapRangesRegex.Match(s);
                ranges.Add(
                    new RangeDescriptor(
                        long.Parse(rangeMatch.Groups["source"].Value),
                        long.Parse(rangeMatch.Groups["destination"].Value),
                        long.Parse(rangeMatch.Groups["range"].Value)
                    )
                );
            }
            maps.Add(new Map(currentMapName, ranges.ToArray()));
            if (s is null)
            {
                break;
            }
        }
        return new Almanac(maps.ToArray(), seeds.ToArray());
    }
}

record RangeDescriptor(long Source, long Destination, long Range);
record Map(string Name, RangeDescriptor[] Ranges);
record Almanac(Map[] Maps, long[] Seeds);
