using System.Data;
using System.Text.RegularExpressions;

namespace _8;

internal static class Program
{
    private static readonly Regex s_GraphNodes = new Regex(@"^(?<start>.*) = \((?<left>.*), (?<right>.*)\)$");
    
    public static void Main(string[] args)
    {
        var input = ParseInput(args);
        //Console.WriteLine(Solve1(input));
        // solve2 is too slow, we need to find cycles and least common multiplier
        //Console.WriteLine(Solve2(input));
        
        var values = input
            .Graph
            .Nodes
            .Keys
            .Where(k => k[^1] == 'A')
            .Select(s => FindLoopLengths(input, s))
            .ToArray();
        Console.WriteLine();
        foreach (var n in values)
        {
            Console.Write("{0} ", n);
        }
        Console.WriteLine();
        Console.WriteLine(values.Aggregate<long>(Lcm));
    }

    private static long Gcd(long a, long b)
    {
        while (a > 0)
        {
            var tmp = a;
            a = b % a;
            b = tmp;
        }

        return b;
    }

    private static long Lcm(long a, long b)
    {
        return a * b / Gcd(a, b);
    }

    private static int Solve1(Map map)
    {
        var currentNode = map.Graph.Start;
        for (var i = 0; i < int.MaxValue; ++i)
        {
            var currentDirection = i % map.Directions.Length;
            if (currentNode is null)
            {
                throw new DataException("Invalid path found");
            }
            if (currentNode.Name == "ZZZ")
            {
                return i;
            }

            currentNode = map.Directions[currentDirection] == Direction.Left
                ? map.Graph.Nodes[currentNode.Left]
                : map.Graph.Nodes[currentNode.Right];
        }

        throw new DataException("No path found");
    }

    private static Node[] ComputeStartingLocations(Map map) =>
        map
            .Graph
            .Nodes
            .Where(kv => kv.Key.EndsWith('A'))
            .Select(kv => kv.Value)
            .ToArray();

    private static bool IsFinished(Node[] locations) =>
        locations.All(n => n.Name[2] == 'Z');
    
    private static long Solve2(Map map)
    {
        var locations = ComputeStartingLocations(map);
        for (var i = 0L; i < long.MaxValue; ++i)
        {
            var currentDirection = i % map.Directions.Length;
            if (IsFinished(locations))
            {
                return i;
            }

            for (var next = 0; next < locations.Length; ++next)
            {
                locations[next] = map.Directions[currentDirection] == Direction.Left
                    ? map.Graph.Nodes[locations[next].Left]
                    : map.Graph.Nodes[locations[next].Right];
            }
        }
        throw new DataException("No path found");
    }

    private static long FindLoopLengths(Map map, string start)
    {
        var visited = new HashSet<(string name, int pos)>();
        var path = new List<string>();
        var current = map.Graph.Nodes[start];
        Console.WriteLine("-----------------------------------");
        Console.WriteLine("--- {0}", start);
        Console.WriteLine("-----------------------------------");
        for (var i = 0; i < int.MaxValue; ++i)
        {
            var pos = i % map.Directions.Length;
            if (visited.Contains((current.Name, pos)))
            {
                var firstEncounter = path.IndexOf(current.Name);
                Console.WriteLine("Hops: {0}", path.Count);
                Console.WriteLine("Count end nodes: {0}", path.Count(e => e[^1] == 'Z'));
                Console.WriteLine("Cycle node: {0}", current.Name);
                Console.WriteLine("First encounter at: {0}", firstEncounter);
                Console.WriteLine("Cycle length: {0}", path.Count - firstEncounter);
                return path.Count - firstEncounter;
            }
            path.Add(current.Name);
            visited.Add((current.Name, pos));
            current = map.Directions[pos] == Direction.Left
                ? map.Graph.Nodes[current.Left]
                : map.Graph.Nodes[current.Right];
        }

        return -1;
    }

    private static Map ParseInput(string[] args)
    {
        var inputSource =
            args.Length > 0
                ? new StreamReader(File.OpenRead(args[0]))
                : Console.In;
        
        var s = inputSource.ReadLine();
        if (s is null || s.Length == 0)
        {
            throw new InvalidDataException("Cannot parse directions");
        }
        var directions = s.Select(d => (Direction)d).ToArray();

        // read an empty line
        s = inputSource.ReadLine();
        if (s is null)
        {
            throw new InvalidDataException("Map does not contain graph");
        }
        
        // read graph
        Node? startNode = null;
        var nodes = new Dictionary<string, Node>();
        while (true)
        {
            s = inputSource.ReadLine();
            if (s is null || s.Length == 0)
            {
                break;
            }

            var match = s_GraphNodes.Match(s);
            var name = match.Groups["start"].Value;
            var left = match.Groups["left"].Value;
            var right = match.Groups["right"].Value;

            var node = new Node(name, left, right);
            nodes[name] = node;
                
            if (name == "AAA")
            {
                startNode = node;
            }
        }

        if (startNode is null)
        {
            // second case
            startNode = nodes.Values.First();
        }
        return new Map(directions, new Graph(startNode, nodes));
    }
}

record Node(string Name, string Left, string Right);
record Graph(Node Start, Dictionary<string, Node> Nodes);

record Map(Direction[] Directions, Graph Graph);
enum Direction
{
    Left = 'L',
    Right = 'R'
}