namespace _11;
using Graph = List<List<char>>;
using Coordinate = (int x, int y);

internal static class Program
{
    public static void Main1(string[] args)
    {
        var space = ExpandSpace(ParseInput(args));
        Console.WriteLine(Solve1(space));
    }

    private static int Solve1(Graph input) =>
        GetGalaxyPairs(input).Sum(x => ComputeDistance(x.start, x.end));

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

    private static int DistanceLowSlope(int x0, int y0, int x1, int y1)
    {
        var dx = x1 - x0;
        var dy = y1 - y0;
        var yi = 1;
        if (dy < 0)
        {
            yi = -1;
            dy = -dy;
        }

        var d = 2 * dy - dx;
        var y = y1;
        
        var steps = -1;
        var oldX = x0;
        var oldY = y0;

        for (var x = x0; x <= x1; ++x)
        {
            ++steps;
            if (x != oldX && y != oldY)
            {
                ++steps;
            }

            oldX = x;
            oldY = y;

            if (d > 0)
            {
                y = y + yi;
                d += 2 * (dy - dx);
            }
            else
            {
                d += 2 * dy;
            }
        }

        return steps;
    }

    private static int DistanceHighSlope(int x0, int y0, int x1, int y1)
    {
        var dx = x1 - x0;
        var dy = y1 - y0;
        var xi = 1;
        if (dx < 0)
        {
            xi = -1;
            dx = -dx;
        }

        var d = 2 * dx - dy;
        var x = x0;

        int oldX = x0, oldY = y0, steps = -1;
        
        for (var y = y0; y <= y1; ++y)
        {
            ++steps;
            if (oldY != y && oldX != x)
            {
                ++steps;
            }

            oldY = y;
            oldX = x;

            if (d > 0)
            {
                x += xi;
                d += 2 * (dx - dy);
            }
            else
            {
                d += 2 * dx;
            }
        }

        return steps;
    }
    
    private static int ComputeDistance(Coordinate start, Coordinate end)
    {
        // https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm

        if (Math.Abs(end.y - start.y) < Math.Abs(end.x - start.x))
        {
            return start.x > end.x 
                ? DistanceLowSlope(end.x, end.y, start.x, start.y) 
                : DistanceLowSlope(start.x, start.y, end.x, end.y);
        }

        return start.y > end.y
            ? DistanceHighSlope(end.x, end.y, start.x, start.y)
            : DistanceHighSlope(start.x, start.y, end.x, end.y);
    }
}