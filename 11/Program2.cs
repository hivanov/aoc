namespace _11;
using Graph = List<List<char>>;
using Coordinate = (int x, int y);

internal static class Program2
{
    private const long SpaceExpand = 1000000 - 1;
    private const long NormalSpace = 1;
    public static void Main(string[] args)
    {
        var space = ExpandSpace(ParseInput(args));
        Console.WriteLine(Solve(space));
    }

    private static long Solve(Graph input) =>
        GetGalaxyPairs(input)
            .GroupBy(x => x.start)
            .AsParallel()
            .Select(g => DistanceFromNode(input, g.Key, g.Select(x => x.end)))
            .Sum();

    private static Graph ParseInput(string[]? args)
    {
        var reader = (args is null || args.Length == 0)
            ? Console.In
            : new StreamReader(File.OpenRead(args[0]));
        var result = new Graph();
        while (true)
        {
            var s = reader.ReadLine();
            if (s is null || s.Length == 0)
            {
                return result;
            }
            result.Add([..s]);
        }
    }

    private static Graph ExpandSpace(Graph input)
    {
        var hasGalaxiesInCol = new bool[input[0].Count];
        for (var y = 0; y < input.Count; ++y)
        {
            var hasGalaxiesInRow = false;
            for (var x = 0; x < input[y].Count; ++x)
            {
                if (input[y][x] == '#') // galaxy
                {
                    hasGalaxiesInRow = true;
                    hasGalaxiesInCol[x] = true;
                }
            }

            if (!hasGalaxiesInRow)
            {
                input.Insert(y, Enumerable.Repeat('e', input[y].Count).ToList());
                y+= 1;
            }
        }

        var indexIncrease = -1;
        for (var x = 0; x < hasGalaxiesInCol.Length; ++x)
        {
            if (hasGalaxiesInCol[x])
            {
                continue;
            }

            ++indexIncrease;
            foreach (var y in input)
            {
                y.Insert(x + indexIncrease, 'e');
            }
        }

        return input;
    }

    private static IEnumerable<Coordinate> GetGalaxyCoordinates(Graph input)
    {
        for (var y = 0; y < input.Count; ++y)
        {
            for (var x = 0; x < input[y].Count; ++x)
            {
                if (input[y][x] == '#')
                {
                    yield return (x, y);
                }
            }
        }
    }

    private static IEnumerable<(Coordinate start, Coordinate end)> GetGalaxyPairs(Graph input)
    {
        var allGalaxies = GetGalaxyCoordinates(input).ToArray();
        for (var start = 0; start < allGalaxies.Length; ++start)
        {
            for (var end = start + 1; end < allGalaxies.Length; ++end)
            {
                yield return (allGalaxies[start], allGalaxies[end]);
            }
        }
    }

    private static long DistanceFromNode(Graph input, Coordinate start, IEnumerable<Coordinate> ends)
    {
        var maxValue = long.MaxValue / 2;
        var distances = new Dictionary<Coordinate, long>();
        var visited = new HashSet<Coordinate>();
        for (var y = 0; y < input.Count; ++y)
        {
            for (var x = 0; x < input[y].Count; ++x)
            {
                var v = (x, y);
                distances[v] = maxValue;
            }
        }

        distances[start] = 0;
        var allVerticesCount = input.Count * input[0].Count;
        while (visited.Count < allVerticesCount - 1)
        {
            var minDistance = distances
                .Where(c => !visited.Contains(c.Key))
                .MinBy(c => c.Value);
            visited.Add(minDistance.Key);
            foreach (var v in GetNeighbors(input, minDistance.Key).Where(x => !visited.Contains(x)))
            {
                var distanceToV = input[v.y][v.x] == 'e' ? SpaceExpand : NormalSpace;
                var fromUtoV = distances[minDistance.Key] + distanceToV;
                if (fromUtoV < distances[v])
                {
                    distances[v] = fromUtoV;
                }
            }
        }

        return ends.Sum(e => distances[e]);
    }

    private static IEnumerable<Coordinate> GetNeighbors(Graph g, Coordinate x)
    {
        if (x.x > 0)
        {
            yield return (x.x - 1, x.y);
        }

        if (x.y > 0)
        {
            yield return (x.x, x.y - 1);
        }

        if (x.x < g[x.y].Count - 1)
        {
            yield return (x.x + 1, x.y);
        }

        if (x.y < g.Count - 1)
        {
            yield return (x.x, x.y + 1);
        }
    }
}