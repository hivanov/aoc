using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

internal class Program
{
    private static readonly Regex m_ParseRegex = new Regex(@"(?:\s*(?<num>\d+))");
    private static void Main(string[] args)
    {
        //Console.WriteLine(Solve1());
        //Console.WriteLine("---------------");
        Console.WriteLine(Solve2());
    }

    private static long Solve1() =>
        ParseInput()
            .Select(CalculateWinningTimes)
            .Aggregate((current, total) => total * current)
    ;

    private static long Solve2() =>
        CalculateWinningTimes(ParseInput2())
    ;

    private static long CalculateWinningTimes(Game game)
    {
        var d = Math.Sqrt(game.Time * game.Time - 4 * game.Record);
        var t1 = (game.Time - d) / 2.0;
        var t2 = (game.Time + d) / 2.0;
        Console.WriteLine("d={2}\tt1 = {0}\tt2={1}\tt2-t1={3}", t1, t2, d, t2 - t1);
        if (t1 < 0 && t2 > game.Time || t2 < 0 || t1 > game.Time)
        {
            Console.WriteLine("No way");
            return 0;
        }
        if (t1 < 0)
        {
            Console.WriteLine("t1 < 0: {0}", (long)t2);
            return (long)t2;
        }
        if (t2 > game.Time)
        {
            var result = (long)(game.Time - t1);
            Console.WriteLine("t2 > T: {0}", result);
            return result;
        }
        else
        {
            var cT1 = Math.Ceiling(t1);
            var fT2 = Math.Floor(t2);
            var left = ((long)cT1 > t1) ? (long)cT1 : (long)(cT1) + 1;
            var right = ((long)fT2 < t2) ? (long)fT2 : (long)(fT2) - 1;
            var result = long.Max(right - left + 1, 0);
            Console.WriteLine("0 <= t1 <= t2 <= T: left={0}\tright={1}\tresult={2}", left, right, result);
            return result;
        }
    }

    private static IEnumerable<Game> ParseInput()
    {
        var times = new List<int>();
        var records = new List<int>();

        // times
        var s = Console.ReadLine();
        var matches = m_ParseRegex.Matches(s);
        foreach (Match match in matches)
        {
            var time = int.Parse(match.Groups["num"].Value);
            times.Add(time);
        }

        // records
        s = Console.ReadLine();
        matches = m_ParseRegex.Matches(s);
        foreach (Match match in matches)
        {
            var record = int.Parse(match.Groups["num"].Value);
            records.Add(record);
        }
        var minLength = int.Min(times.Count, records.Count);

        for (var i = 0; i < minLength; ++i)
        {
            yield return new Game(times[i], records[i]);
        }
    }

    private static long ReadAndParseLongNum()
    {
        var s = Console.ReadLine();
        long result = 0;
        for (var i = 0; i < s.Length; ++i)
        {
            if (s[i] >= '0' && s[i] <= '9')
            {
                result = 10 * result + s[i] - '0';
            }
        }
        Console.WriteLine(result);
        return result;
    }
    private static Game ParseInput2() =>
        new Game(
            ReadAndParseLongNum(),
            ReadAndParseLongNum()
        )
    ;
}

record Game(long Time, long Record);