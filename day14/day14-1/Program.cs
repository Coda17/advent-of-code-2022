var paths = File.ReadAllLines(args[0])
    .Select(x => x.Split(" -> ")
        .Select(y => (x: int.Parse(y[..y.IndexOf(',')]), y: int.Parse(y[(y.IndexOf(',') + 1)..]))).ToList()).ToArray();

for (var i = 0; i < paths.Length; ++i)
{
    paths[i] = ExpandPath(paths[i]);
}

var (map, start) = BuildMap(paths);
//PrintMap(map);
Console.WriteLine(Simulate(map, start));

// Returns grains of sand poured.
static int Simulate(IReadOnlyList<char[]> map, int start)
{
    var count = 0;
    while (AddSand(map, start))
    {
        ++count;
        //PrintMap(map);
    }

    return count;
}

// Returns true if the sand stops, otherwise; false.
static bool AddSand(IReadOnlyList<char[]> map, int start)
{
    var sand = (x: start, y: 0);
    while (true)
    {
        var canFall = sand.y + 1 < map.Count;
        if (canFall && map[sand.y + 1][sand.x] == '.')
        {
            sand = (sand.x, sand.y + 1);
        }
        else if (canFall && map[sand.y + 1][sand.x - 1] == '.')
        {
            sand = (sand.x - 1, sand.y + 1);
        }
        else if (canFall && map[sand.y + 1][sand.x + 1] == '.')
        {
            sand = (sand.x + 1, sand.y + 1);
        }
        else
        {
            if (sand.y + 1 >= map.Count)
            {
                return false;
            }

            map[sand.y][sand.x] = 'o';
            return true;
        }
    }
}

static List<(int x, int y)> ExpandPath(IReadOnlyList<(int x, int y)> path)
{
    var expanded = new List<(int x, int y)>();
    var i = 0;
    while (i + 1 < path.Count)
    {
        if (path[i].x == path[i + 1].x)
        {
            var min = Math.Min(path[i].y, path[i + 1].y);
            var max = Math.Max(path[i].y, path[i + 1].y);
            expanded.AddRange(Enumerable.Range(min, max - min + 1).Select(y => (path[i].x, y)));
        }
        else if (path[i].y == path[i + 1].y)
        {
            var min = Math.Min(path[i].x, path[i + 1].x);
            var max = Math.Max(path[i].x, path[i + 1].x);
            expanded.AddRange(Enumerable.Range(min, max - min + 1).Select(x => (x, path[i].y)));
        }

        ++i;
    }

    return expanded;
}

static (char[][], int start) BuildMap(List<(int x, int y)>[] paths)
{
    var minX = paths.SelectMany(path => path.Select(coords => coords.x)).Aggregate(int.MaxValue, Math.Min) - 1;
    var maxX = Math.Max(500, paths.SelectMany(path => path.Select(coords => coords.x)).Aggregate(int.MinValue, Math.Max)) + 1;
    const int minY = 0;
    var maxY = paths.SelectMany(path => path.Select(coords => coords.y)).Aggregate(int.MinValue, Math.Max);

    var map = new char[maxY + 1][];
    for (var y = minY; y <= maxY; ++y)
    {
        var row = new char[maxX - minX + 1];
        for (var x = 0; x < row.Length; ++x)
        {
            var currentX = x + minX;
            var currentY = y;
            if (currentX == 500 && y == 0)
            {
                row[x] = '+';
            }
            else if (paths.Any(path => path.Contains((currentX, currentY))))
            {
                row[x] = '#';
            }
            else
            {
                row[x] = '.';
            }
        }

        map[y] = row;
    }

    return (map, 500 - minX);
}

static void PrintMap(IReadOnlyList<char[]> map)
{
    for (var y = 0; y < map.Count; ++y)
    {
        Console.Write($"{y} ");

        for (var x = 0; x < map[y].Length; ++x)
        {
            Console.Write(map[y][x]);
        }

        Console.Write(Environment.NewLine);
    }

    Console.Write(Environment.NewLine);
}