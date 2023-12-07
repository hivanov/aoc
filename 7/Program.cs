internal class Program
{
    private const int HandSize = 13;
    private static readonly HandComparer s_HandComparer = new();
    private static readonly HandComparer2 s_HandComparer2 = new();

    private static void Main(string[] args)
    {
        var input = ParseInput(args).ToArray();
        Console.WriteLine(Solve1(input));
        Console.WriteLine(Solve2(input));
    }

    private static long Solve1(IEnumerable<Hand> input) =>
        input
            .OrderBy(hand => hand, s_HandComparer)
            .ThenBy(hand => hand.Bid)
            .Select((hand, index) => (index + 1) * hand.Bid)
            .Sum()
    ;

    private static long Solve2(IEnumerable<Hand> input) =>
        input
            .OrderBy(hand => hand, s_HandComparer2)
            .ThenBy(hand => hand.Bid)
            .Select((hand, index) => (index + 1) * hand.Bid)
            .Sum()
    ;
    
    private static Hand ParseHand(string input)
    {
        var split = input.Split(' ');
        var parsed = ConvertHand(split[0]);
        var bid = long.Parse(split[1]);
        var hand = new Hand(split[0], parsed, bid);
        return hand;
    }

    private static IEnumerable<Hand> ParseInput(string[] args)
    {
        string? s;
        var reader = args.Length > 0 
            ? File.OpenText(args[0]) 
            : Console.In;

        while (true)
        {
            s = reader.ReadLine();
            if (s is null || s.Length == 0)
            {
                break;
            }
            yield return ParseHand(s);
        }
    }

    private static (char c, int rank)[] ConvertHand(string hand) =>
        hand
            .GroupBy(c => c)
            .Select(g => (g.Key, g.Count()))
            .OrderByDescending(g => g.Item2)
            .ToArray();

    private class HandComparer : IComparer<Hand>
    {
        int IComparer<Hand>.Compare(Hand? a, Hand? b)
        {
            // is has null?
            if (a is null)
            {
                if (b is null)
                {
                    return 0;
                }
                return -1;
            }

            if (b is null)
            {
                return 1;
            }

            // greater rank wins
            var aRank = ComputeHandType(a);
            var bRank = ComputeHandType(b);
            var rankCompare = Comparer<HandType>
                .Default
                .Compare(aRank, bRank);
            if (rankCompare != 0)
            {
                return rankCompare;
            }

            // higher card in order wins
            var intComparer = Comparer<int>.Default;
            for (var i = 0; i < 5; ++i)
            {
                var r = intComparer.Compare(Rank(a.Original[i]), Rank(b.Original[i]));
                if (r != 0)
                {
                    return r;
                }
            }

            // all equal
            return 0;
        }

        private static HandType ComputeHandType(Hand hand)
        {
            return hand.Computed[0].rank switch
            {
                5 => HandType.FiveOfKind,
                4 => HandType.FourOfKind,
                3 when hand.Computed[1].rank == 2 => HandType.FullHouse,
                3 => HandType.ThreeOfKind,
                2 when hand.Computed[1].rank == 2 => HandType.TwoPair,
                2 => HandType.OnePair,
                _ => HandType.HighCard
            };
        }

        private static int Rank(char card) =>
            card switch
            {
                'A' => 12,
                'K' => 11,
                'Q' => 10,
                'J' => 9,
                'T' => 8,
                '9' => 7,
                '8' => 6,
                '7' => 5,
                '6' => 4,
                '5' => 3,
                '4' => 2,
                '3' => 1,
                '2' => 0,
                _ => throw new ArgumentException($"Invalid card: {card}", nameof(card))
            }
        ;
    }

    private class HandComparer2 : IComparer<Hand>
    {
        public int Compare(Hand? a, Hand? b)
        {
            // null comparison
            if (a is null)
            {
                if (b is null)
                {
                    return 0;
                }

                return -1;
            }
            if (b is null)
            {
                return 1;
            }

            // greater rank wins
            var aRank = ComputeHandType(a);
            var bRank = ComputeHandType(b);
            var rankCompare = Comparer<HandType>
                .Default
                .Compare(aRank, bRank);
            if (rankCompare != 0)
            {
                return rankCompare;
            }

            // higher card in order wins
            var intComparer = Comparer<int>.Default;
            for (var i = 0; i < 5; ++i)
            {
                var r = intComparer.Compare(Rank(a.Original[i]), Rank(b.Original[i]));
                if (r != 0)
                {
                    return r;
                }
            }

            // all equal
            return 0;
        }
        
        private static int Rank(char card) =>
            card switch
            {
                'A' => 12,
                'K' => 11,
                'Q' => 10,
                'T' => 9,
                '9' => 8,
                '8' => 7,
                '7' => 6,
                '6' => 5,
                '5' => 4,
                '4' => 3,
                '3' => 2,
                '2' => 1,
                'J' => 0,
                _ => throw new ArgumentException($"Invalid card: {card}", nameof(card))
            }
        ;
        
        private static HandType ComputeHandType(Hand hand)
        {
            var jRank = hand.Computed.FirstOrDefault(c => c.c == 'J').rank;
            switch (jRank)
            {
                case 0:
                    return hand.Computed[0].rank switch
                    {
                        5 => HandType.FiveOfKind,
                        4 => HandType.FourOfKind,
                        3 when hand.Computed[1].rank == 2 => HandType.FullHouse,
                        3 => HandType.ThreeOfKind,
                        2 when hand.Computed[1].rank == 2 => HandType.TwoPair,
                        2 => HandType.OnePair,
                        _ => HandType.HighCard
                    };
                case 5:
                // 4j + whatever
                case 4:
                // 3j + 2x
                case 3 when hand.Computed[1].rank == 2:
                    // all j
                    return HandType.FiveOfKind;
                // 3j + whatever
                case 3:
                    return HandType.FourOfKind;
                case 2 when hand.Computed[0].rank == 3:
                    // 3x + 2j
                    return HandType.FiveOfKind;
                case 2 when hand.Computed[0].rank == 2 && hand.Computed[0].c != 'J':
                // 2J + 2x
                case 2 when hand.Computed[1].rank == 2 && hand.Computed[1].c != 'J':
                    // 2x + 2J
                    return HandType.FourOfKind;
                case 2:
                    return HandType.ThreeOfKind;
                default:
                    // jRank == 1
                    return hand.Computed[0].rank switch
                    {
                        4 => HandType.FiveOfKind,
                        3 => HandType.FourOfKind,
                        2 when hand.Computed[1].rank == 2 => HandType.FullHouse,
                        2 => HandType.ThreeOfKind,
                        1 => HandType.OnePair,
                        _ => HandType.HighCard
                    };
            }
        }
    }
}


enum HandType
{
    FiveOfKind = 6,
    FourOfKind = 5,
    FullHouse = 4,
    ThreeOfKind = 3,
    TwoPair = 2,
    OnePair = 1,
    HighCard = 0
}

record Hand(string Original, (char c, int rank)[] Computed, long Bid);