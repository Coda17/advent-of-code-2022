var lines = File.ReadAllLines(args[0]);
var fullOverlaps = 0;
foreach (var line in lines)
{
    var pairs = line.Split(',');
    var ranges = pairs.Select(x => x.Split('-').Select(int.Parse).ToArray()).ToArray();

    if ((ranges[0][0] >= ranges[1][0] && ranges[0][1] <= ranges[1][1]) ||
        (ranges[1][0] >= ranges[0][0] && ranges[1][1] <= ranges[0][1]))
    {
        ++fullOverlaps;
    }
}

Console.WriteLine(fullOverlaps);