using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

internal class Program
{
    private static readonly Regex CardRegex = new Regex(@"^Card\s+(?<num>\d+):\s*(?:(?<win>\d+)\s+)+\|(?:\s+(?<have>\d+))+$");
    private static void Main(string[] args)
    {
        //Console.WriteLine(Solve1());
        Console.WriteLine(Solve2());
    }

    private static IEnumerable<Card> ParseInput()
    {
        while (true)
        {
            var s = Console.ReadLine();
            if (s is null || s.Length == 0)
            {
                yield break;
            }
            yield return ParseCard(s);
        }
    }
    
    private static int CountMatchingNumbers(Card card) =>
        card
            .Have
            .Where(
                h => 
                    card
                        .Winning
                        .Contains(h))
            .Count()
    ;

    private static int ComputeScore(Card card) =>
        int.Max(1 << (CountMatchingNumbers(card) - 1), 0);
    
    private static Card ParseCard(string s)
    {
        var match = CardRegex.Match(s);
        var win = new HashSet<int>();
        var have = new List<int>();
        foreach (Capture data in match.Groups["win"].Captures)
        {
            win.Add(int.Parse(data.Value));
        }
        foreach (Capture data in match.Groups["have"].Captures)
        {
            have.Add(int.Parse(data.Value));
        }
        var cardId = int.Parse(match.Groups["num"].Value);
        return new Card(cardId, win, have.ToArray());
    }

    private static int Solve1()
    {
        var score = 0;
        foreach (var card in ParseInput())
        {
            score += ComputeScore(card);
        }
        return score;
    }

    private static int Solve2()
    {
        var allCards = ParseInput().ToArray();
        var result = new ConcurrentDictionary<int, int>();

        foreach (var card in allCards)
        {
            var current = result.AddOrUpdate(
                card.Number,
                1,
                (_, oldV) => oldV + 1
            );
            var matching = CountMatchingNumbers(card);
            for (var i = card.Number + 1; i <= card.Number + matching; ++i)
            {
                var m = result.AddOrUpdate(
                    i,
                    current,
                    (_, oldV) => oldV + current
                );
            }
        }
        return result.Values.Sum();
    }
}

record Card(int Number, HashSet<int> Winning, int[] Have);
