var lines = File.ReadAllLines(args[0]).Select((x, i) => (val: x, index: i)).ToArray();

var start = lines.Where(x => x.val.Contains('S'))
    .Select(x => (x: x.val.IndexOf('S'), y: x.index))
    .First();

var goal = lines.Where(x => x.val.Contains('E'))
    .Select(x => (x: x.val.IndexOf('E'), y: x.index))
    .First();

var map = lines.Select(x => x.val.ToCharArray()).ToArray();

map[start.y][start.x] = 'a';
map[goal.y][goal.x] = 'z';

var path = AStar(map, start, goal);
for (var col = 0; col < map.Length; ++col)
{
    for (var row = 0; row < map[col].Length; ++row)
    {
        var visited = path.IndexOf((row, col));
        var indicator = '.';
        if (visited != -1)
        {
            if (visited == path.Count - 1)
            {
                indicator = 'E';
            }
            else if (path[visited].x > path[visited + 1].x)
            {
                indicator = '<';
            }
            else if (path[visited].x < path[visited + 1].x)
            {
                indicator = '>';
            }
            else if (path[visited].y > path[visited + 1].y)
            {
                indicator = '^';
            }
            else if (path[visited].y < path[visited + 1].y)
            {
                indicator = 'v';
            }
        }

        Console.Write(indicator);
    }

    Console.Write('\n');
}

Console.WriteLine(path.Count - 1);

static IList<(int x, int y)> AStar(IReadOnlyList<char[]> map, (int x, int y) start, (int x, int y) goal)
{
    var openSet = new PriorityQueue<(int, int), int>(new[] { (start, 0) });
    var cameFrom = new Dictionary<(int x, int y), (int x, int y)>();

    var gScore = new Dictionary<(int, int), int>
    {
        [start] = 0
    };

    var fScore = new Dictionary<(int, int), int>
    {
        [start] = H(start, goal)
    };

    while (openSet.TryDequeue(out var current, out _))
    {
        if (current == goal)
        {
            return ReconstructPath(cameFrom, current);
        }

        var neighbors = Neighbors(map, current);
        foreach (var neighbor in neighbors)
        {
            var tentativeGScore = (gScore.ContainsKey(current) ? gScore[current] : int.MaxValue) + 1;
            if (tentativeGScore < (gScore.ContainsKey(neighbor) ? gScore[neighbor] : int.MaxValue))
            {
                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = tentativeGScore + H(neighbor, goal);
                if (openSet.UnorderedItems.All(x => x.Element != neighbor))
                {
                    openSet.Enqueue(neighbor, fScore.ContainsKey(neighbor) ? fScore[neighbor] : int.MaxValue);
                }
            }
        }
    }

    return new List<(int x, int y)>();
}

static IList<(int x, int y)> ReconstructPath(IDictionary<(int x, int y), (int x, int y)> cameFrom,
    (int x, int y) current)
{
    var totalPath = new List<(int x, int y)> { current };
    while (cameFrom.ContainsKey(current))
    {
        current = cameFrom[current];
        totalPath.Insert(0, current);
    }

    return totalPath;
}

static int H((int x, int y) node, (int x, int y) goal) =>
    Math.Abs(goal.x - node.x) + Math.Abs(goal.y - node.y);

static IEnumerable<(int x, int y)> Neighbors(IReadOnlyList<char[]> map, (int x, int y) node)
{
    var height = map[node.y][node.x];
    if (node.x > 0 && map[node.y][node.x - 1] - height <= 1)
    {
        yield return (node.x - 1, node.y);
    }

    if (node.y > 0 && map[node.y - 1][node.x] - height <= 1)
    {
        yield return (node.x, node.y - 1);
    }

    if (node.x + 1 < map[0].Length && map[node.y][node.x + 1] - height <= 1)
    {
        yield return (node.x + 1, node.y);
    }

    if (node.y + 1 < map.Count && map[node.y + 1][node.x] -  height <= 1)
    {
        yield return (node.x, node.y + 1);
    }
}