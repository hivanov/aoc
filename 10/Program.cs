using System.Diagnostics.CodeAnalysis;

namespace _10;

internal abstract class Program
{
    public static void Main(string[] args)
    {
        var input = ParseInput(args);
        var digraph = DigraphOnly(input);
        // 1
        Console.WriteLine(BreadthFirstVisit(digraph));
        // 2:
        Console.WriteLine(Solve2(OnlyMainLoop(input)));
    }

    private static Graph OnlyMainLoop(Graph g)
    {
        foreach (var p in g.Vertices[g.Start])
        {
            Console.WriteLine("\n-------------");
            var v = Visit(p, g, new HashSet<Point>() {  });
            if (v is null)
            {
                continue;
            }
            var vertices = new Dictionary<Point, Point[]>();
            for (var i = 1; i < v.Count; ++i)
            {
                if (vertices.TryGetValue(v[i], out var fromI))
                {
                    vertices[v[i]] = new List<Point>(fromI) { v[i-1] }.ToArray();
                }
                else
                {
                    vertices[v[i]] = new []{ v[i - 1]};
                }

                if (vertices.TryGetValue(v[i - 1], out var fromI1))
                {
                    vertices[v[i - 1]] = new List<Point>(fromI1) { v[i] }.ToArray();
                }
                else
                {
                    vertices[v[i - 1]] = new[] { v[i] };
                }
            }

            return g with
            {
                Vertices = vertices
            };
        }

        return g;
    }
    
    private static List<Point>? Visit(Point node, Graph g, HashSet<Point> visited)
    {
        if (node == g.Start)
        {
            return new List<Point> { node };
        }

        visited.Add(node);
        foreach (var v in g.Vertices[node].Where(b => !visited.Contains(b)))
        {
            var result = Visit(v, g, visited);
            if (result is null)
            {
                visited.Remove(node);
                continue;
            }
            result.Add(node);
            return result;
        }

        return null;
    }

    private static int Solve2(Graph digraph)
    {
        var result = 0;
        for (var y = 0; y < digraph.MaxY; ++y)
        {
            var inside = false;
            for (var x = 0; x < digraph.MaxX; ++x)
            {
                if (digraph.Vertices.TryGetValue(new Point(x, y), out var adj))
                {
                    if (adj.Contains(new Point(x, y - 1)))
                    {
                        Console.Write('+');
                        inside = !inside;
                    }
                    else
                    {
                        Console.Write('-');
                    }
                }
                else
                {
                    if (inside)
                    {
                        ++result;
                        Console.Write('.');
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
            }
            Console.WriteLine();
        }

        Console.WriteLine();
        return result;
    }

    private static int BreadthFirstVisit(Graph g)
    {
        var queue = new Queue<(Point Node, int Level)>();
        queue.Enqueue((g.Start, 0));
        var visit = new Dictionary<Point, int>();
        while (queue.Count > 0)
        {
            var (node, level) = queue.Dequeue();
            if (!visit.TryAdd(node, level))
            {
                continue;
            }
            if (!g.Vertices.TryGetValue(node, out var adjacent))
            {
                continue;
            }

            foreach (var point in adjacent)
            {
                queue.Enqueue((point, level + 1));
            }
        }

        return visit.Values.Max();
    }

    private static Graph DigraphOnly(Graph g)
    {
        var newVertices = new List<(Point start, Point end)>();
        foreach (var kv in g.Vertices)
        {
            foreach (var adjacent in kv.Value)
            {
                if (!g.Vertices.TryGetValue(adjacent, out var edges))
                {
                    continue;
                }

                if (!edges.Contains(kv.Key))
                {
                    continue;
                }
                newVertices.Add((kv.Key, adjacent));
                newVertices.Add((adjacent, kv.Key));
            }
        }

        return g with 
        { 
            Vertices = new Dictionary<Point, Point[]>(
                newVertices
                    .GroupBy(p => p.start)
                    .Select(p =>
                        new KeyValuePair<Point, Point[]>(
                            p.Key, 
                            p.Select(v => v.end).ToArray()))) 
        };
    }

    private static Graph ParseInput(string[]? args)
    {
        var reader = (args is not null && args.Length > 0)
            ? new StreamReader(File.OpenRead(args[0]))
            : Console.In;
        var vertices = new Dictionary<Point, Point[]>();
        Point? startNode = null;
        var maxX = 0;
        for (var y = 0;; ++y)
        {
            var s = reader.ReadLine();
            if (s is null || s.Length == 0)
            {
                if (startNode is null)
                {
                    throw new InvalidDataException("No start node");
                }

                var pts = new List<Point>();
                if (startNode.X > 0)
                {
                    var p = new Point(startNode.X - 1, startNode.Y);
                    if (vertices.TryGetValue(p, out var v) && v.Contains(startNode))
                    {
                        pts.Add(p);
                    }
                }

                if (startNode.X < maxX - 1)
                {
                    var p = new Point(startNode.X + 1, startNode.Y);
                    if (vertices.TryGetValue(p, out var v) && v.Contains(startNode))
                    {
                        pts.Add(p);
                    }
                }

                if (startNode.Y < y - 1)
                {
                    var p = new Point(startNode.X, startNode.Y + 1);
                    if (vertices.TryGetValue(p, out var v) && v.Contains(startNode))
                    {
                        pts.Add(p);
                    }
                }

                if (startNode.Y > 0)
                {
                    var p = new Point(startNode.X, startNode.Y - 1);
                    if (vertices.TryGetValue(p, out var v) && v.Contains(startNode))
                    {
                        pts.Add(p);
                    }
                }

                vertices[startNode] = pts.ToArray(); 
                return new Graph(startNode, vertices, maxX, y);
            }

            maxX = Math.Max(maxX, s.Length);
            for (var x = 0; x < s.Length; ++x)
            {
                vertices.Add(
                    new Point(x, y),
                    s[x] switch
                    {
                        '|' => new [] { new Point(x, y - 1), new Point(x, y + 1) }, // ns
                        '-' => new [] { new Point(x - 1, y), new Point(x + 1, y) }, // we
                        'L' => new [] { new Point(x, y - 1), new Point(x + 1, y) }, // ne
                        'J' => new [] { new Point(x, y - 1), new Point(x - 1, y) }, // nw
                        '7' => new [] { new Point(x, y + 1), new Point(x - 1, y) }, // sw
                        'F' => new [] { new Point(x, y + 1), new Point(x + 1, y) }, // se
                        _ => Array.Empty<Point>()
                    }
                );
                if (s[x] == 'S')
                {
                    startNode = new Point(x, y);
                }
            }
        }
    }
}

[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global")]
record Point(int X, int Y);
record Graph(Point Start, Dictionary<Point, Point[]> Vertices, int MaxX, int MaxY);