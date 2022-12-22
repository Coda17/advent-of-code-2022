using System.Text.RegularExpressions;

var regex = new Regex("x=(?'x1'-?\\d+), y=(?'y1'-?\\d+).+x=(?'x2'-?\\d+), y=(?'y2'-?\\d+)");
var sensors = File.ReadAllLines(args[0]).Select(x =>
{
    var match = regex.Match(x);
    var sensor = (x: int.Parse(match.Groups["x1"].Value), y: int.Parse(match.Groups["y1"].Value));
    var beacon = (x: int.Parse(match.Groups["x2"].Value), y: int.Parse(match.Groups["y2"].Value));
    var distance = ManhattanDistance(sensor, beacon);
    return (sensor, distance);
}).ToArray();

const long max = 4000000;

var range = new Range(0, (int) max);
for (var y = 0; y <= max; ++y)
{
    var current = y;
    var beaconsReach = sensors.Select(sensor =>
    {
        var distanceFromRow = Math.Abs(sensor.sensor.y - current);
        if (distanceFromRow > sensor.distance)
        {
            return new Range(0, -1);
        }

        var distanceLeft = sensor.distance - distanceFromRow;
        return new Range(sensor.sensor.x - distanceLeft, sensor.sensor.x + distanceLeft);
    });

    var distressBeacon = LocateDistressBeacon(beaconsReach, range);
    if (distressBeacon is not null)
    {
        Console.WriteLine($"Distress beacon located at ({distressBeacon}, {y}) with a tuning frequency of {distressBeacon * 4000000L + y}");
        break;
    }
}

static int? LocateDistressBeacon(IEnumerable<Range> ranges, Range range)
{
    ranges = ranges.Select(x => x.Intersect(range)).Where(x => !x.IsEmpty).OrderBy(x => x.Min).ThenBy(x => x.Max);

    var max = range.Min - 1;
    foreach (var r in ranges)
    {
        if (max + 1 < r.Min)
        {
            return max + 1;
        }

        max = Math.Max(r.Max, max);
    }

    return max < range.Max ? max + 1 : null;
}

static int ManhattanDistance((int x, int y) p1, (int x, int y) p2) => Math.Abs(p1.x - p2.x) + Math.Abs(p1.y - p2.y);

internal record struct Range(int Min, int Max)
{
    public IEnumerable<int> Values => IsEmpty ? Enumerable.Empty<int>() : Enumerable.Range(Min, Max - Min + 1);
    public bool IsEmpty => Min > Max;
    public bool Overlaps(Range other) => !IsEmpty && !other.IsEmpty && Min <= other.Max && Max >= other.Min;

    public Range Intersect(Range other) =>
        Overlaps(other) ? new Range(Math.Max(Min, other.Min), Math.Min(Max, other.Max)) : new Range(0, -1);
}