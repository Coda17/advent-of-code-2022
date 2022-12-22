using System.Text.RegularExpressions;

var regex = new Regex("x=(?'x1'-?\\d+), y=(?'y1'-?\\d+).+x=(?'x2'-?\\d+), y=(?'y2'-?\\d+)");
var sensors = File.ReadAllLines(args[0]).Select(x =>
{
    var match = regex.Match(x);
    var sensor = (x: long.Parse(match.Groups["x1"].Value), y: long.Parse(match.Groups["y1"].Value));
    var beacon = (x: long.Parse(match.Groups["x2"].Value), y: long.Parse(match.Groups["y2"].Value));
    var distance = ManhattanDistance(sensor, beacon);
    return (sensor, distance);
}).ToArray();

var notBeacons = new HashSet<long>();
foreach (var pair in sensors)
{
    const long y = 2000000L;
    var distanceFromRow = Math.Abs(pair.sensor.y - y);
    var distanceLeft = pair.distance - distanceFromRow;
    for (var x = distanceLeft * -1; x < distanceLeft; ++x)
    {
        notBeacons.Add(pair.sensor.x + x);
    }
}

Console.WriteLine($"{notBeacons.Count} cannot contain a beacon.");

static long ManhattanDistance((long x, long y) p1, (long x, long y) p2) => Math.Abs(p1.x - p2.x) + Math.Abs(p1.y - p2.y);