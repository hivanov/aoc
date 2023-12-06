string? s;
int total = 0;
do {
    s = Console.ReadLine();
    if (s == null || s.Length == 0) {
        break;
    }

    var addendum = GetNumber(s);
    Console.WriteLine(addendum);
    total += addendum;
} while (true);
Console.WriteLine(total);

partial class Program
{
    private static int GetNumber(string s) {
        var min = new int[10] {
            int.MaxValue,
            int.MaxValue,
            int.MaxValue,
            int.MaxValue,
            int.MaxValue,
            int.MaxValue,
            int.MaxValue,
            int.MaxValue,
            int.MaxValue,
            int.MaxValue,
        };
        var max = new int[10] {
            int.MinValue,
            int.MinValue,
            int.MinValue,
            int.MinValue,
            int.MinValue,
            int.MinValue,
            int.MinValue,
            int.MinValue,
            int.MinValue,
            int.MinValue,
        };
        void n(int pos, string search)
        {
            var v = s.IndexOf(search);
            if (v >= 0)
            {
                if (min[pos] > v)
                {
                    min[pos] = v;
                }
            }
            v = s.LastIndexOf(search);
            if (v >= 0)
            {
                if (max[pos] < v)
                {
                    max[pos] = v;
                }
            }
        }
        n(0, "0");
        n(1, "1");
        n(1, "one");
        n(2, "2");
        n(2, "two");
        n(3, "3");
        n(3, "three");
        n(4, "4");
        n(4, "four");
        n(5, "5");
        n(5, "five");
        n(6, "6");
        n(6, "six");
        n(7, "7");
        n(7, "seven");
        n(8, "8");
        n(8, "eight");
        n(9, "9");
        n(9, "nine");
        var minIndex = int.MaxValue;
        var maxIndex = int.MinValue;
        var minValue = -1;
        var maxValue = -1;
        for (var i = 0; i < 10; ++i)
        {
            if (min[i] < minIndex) 
            {
                minIndex = min[i];
                minValue = i;
            }
            if (max[i] > maxIndex)
            {
                maxIndex = max[i];
                maxValue = i;
            }
        }
        return minValue * 10 + maxValue;
    }

}