using System.Collections.Immutable;
using System.Text;

var fuelRequirements = File.ReadLines(args[0]);

Console.WriteLine(ToSnafu(fuelRequirements.Select(ToDecimal).Sum()));

static long ToDecimal(string s)
{
    var snafuToDigits = new Dictionary<char, int>
    {
        { '2', 2 },
        { '1', 1 },
        { '0', 0 },
        { '-', -1 },
        { '=', -2 }
    }.ToImmutableDictionary();

    var place = Enumerable.Repeat(5, s.Length - 1).Aggregate(1L, (i, x) => i * x);
    var value = 0L;
    foreach (var c in s)
    {
        value += place * snafuToDigits[c];
        place /= 5;
    }

    return value;
}

static string ToSnafu(long l)
{
    var digits = new[] { '=', '-', '0', '1', '2' };
    var sb = new StringBuilder();
    const int radix = 5;
    while (l != 0)
    {
        var div = (l + 2) / radix;
        var mod = (l + 2) % radix;
        sb.Insert(0, digits[mod]);
        l = div;
    }

    return sb.ToString();
}