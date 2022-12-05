var lines = File.ReadAllLines(args[0]);
var overlaps = 0;
foreach (var line in lines)
{
    var pairs = line.Split(',');
    var ranges = pairs.Select(x =>
    {
        var range = x.Split('-').Select(int.Parse).ToArray();
        return Enumerable.Range(range[0], range[1] - range[0] + 1).ToArray();
    }).ToArray();

    overlaps += ranges[0].Intersect(ranges[1]).Any() ? 1 : 0;
}

Console.WriteLine(overlaps);