// See https://aka.ms/new-console-template for more information

using System.Text.RegularExpressions;

internal class Program
{
    private static readonly Regex s_Numbers = new(@"(\s*(?<num>d+))");
    public static void Main(string[] args)
    {
        Console.WriteLine(Solve1(ParseCoefficients(args)));
    }

    private static long Solve1(IEnumerable<int[]> input) =>
        input.Sum(GetNextValue);

    private static long GetNextValue(int[] currentValues)
    {
        if (currentValues.Distinct().Count() <= 1)
        {
            return currentValues[0];
        }
        var x = new int[currentValues.Length - 1];
        for (var i = 0; i < currentValues.Length - 1; ++i)
        {
            x[i] = currentValues[i + 1] - currentValues[i];
        }

        //return GetNextValue(x) + currentValues[^1];
        return currentValues[0] - GetNextValue(x);
    }


    private static IEnumerable<int[]> ParseCoefficients(string[] args)
    {
        var reader = args.Length > 0
            ? new StreamReader(File.OpenRead(args[0]))
            : Console.In;
        while (true)
        {
            var s = reader.ReadLine();
            if (s is null || s.Length == 0)
            {
                yield break;
            }

            yield return s.Split(' ').Select(int.Parse).ToArray();
        }
    }
}
