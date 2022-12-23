using System.Text;

var proposals = File.ReadAllLines(args[0]).Select(x => x.ToCharArray()).ToArray();

const string proposalOrder = "NSWE";

const int rounds = 10;
var proposalIndex = 0;
for (var round = 0; round < rounds; ++round)
{
    var order = proposalOrder.Substring(proposalIndex, proposalOrder.Length - proposalIndex) +
                proposalOrder[..proposalIndex];

    proposals = proposals.Select((row, y) =>
            row.Select((_, x) => ProposeDirection(proposals, x, y, order)).ToArray())
        .ToArray();

    var updated = new char[proposals.Length + 2][];
    var movementFails = new HashSet<(int x, int y)>();
    for (var y = -1; y <= proposals.Length; ++y)
    {
        var line = new StringBuilder();
        for (var x = -1; x <= proposals[0].Length; ++x)
        {
            if (ValidCoordinate(proposals, x, y) && proposals[y][x] == '#')
            {
                line.Append('#');
                continue;
            }

            var elves = ProposedMovementsToThisSquare(proposals, x, y).ToArray();
            if (!elves.Any())
            {
                line.Append('.');
            }
            else if (elves.Length == 1)
            {
                line.Append('#');
            }
            else
            {
                line.Append('.');
                foreach (var elf in elves)
                {
                    movementFails.Add(elf);
                }
            }
        }

        updated[y + 1] = line.ToString().ToCharArray();
    }

    foreach (var (x, y) in movementFails)
    {
        // The +1s compensate for the map expansion.
        updated[y + 1][x + 1] = '#';
    }

    proposals = ReduceMap(updated);
    proposalIndex = (proposalIndex + 1) % proposalOrder.Length;
}

Console.WriteLine(proposals.SelectMany(y => y.Where(x => x == '.')).Count());

char ProposeDirection(IReadOnlyList<char[]> map, int x, int y, string order)
{
    if (map[y][x] == '.')
    {
        return '.';
    }

    if (ShouldDoNothing(map, x, y))
    {
        return '#';
    }

    foreach (var direction in order.Where(direction => CanMoveDirection(map, x, y, direction)))
    {
        return direction;
    }

    return '#';
}

bool ShouldDoNothing(IReadOnlyList<char[]> map, int x, int y) =>
    OpenSpot(map, x - 1, y - 1) && OpenSpot(map, x, y - 1) && OpenSpot(map, x + 1, y - 1) &&
    OpenSpot(map, x - 1, y) && OpenSpot(map, x + 1, y) &&
    OpenSpot(map, x - 1, y + 1) && OpenSpot(map, x, y + 1) && OpenSpot(map, x + 1, y + 1);

bool CanMoveDirection(IReadOnlyList<char[]> map, int x, int y, char direction) => direction switch
{
    'N' => OpenSpot(map, x - 1, y - 1) && OpenSpot(map, x + 1, y - 1) && OpenSpot(map, x, y - 1),
    'S' => OpenSpot(map, x - 1, y + 1) && OpenSpot(map, x + 1, y + 1) && OpenSpot(map, x, y + 1),
    'W' => OpenSpot(map, x - 1, y - 1) && OpenSpot(map, x - 1, y + 1) && OpenSpot(map, x - 1, y),
    'E' => OpenSpot(map, x + 1, y - 1) && OpenSpot(map, x + 1, y + 1) && OpenSpot(map, x + 1, y),
    _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Invalid direction")
};

static bool OpenSpot(IReadOnlyList<char[]> map, int x, int y) => !ValidCoordinate(map, x, y) || map[y][x] == '.';

IEnumerable<(int x, int y)> ProposedMovementsToThisSquare(IReadOnlyList<char[]> map, int x, int y)
{
    if (ValidCoordinate(map, x, y - 1) && map[y - 1][x] == 'S')
    {
        yield return (x, y - 1);
    }

    if (ValidCoordinate(map, x, y + 1) && map[y + 1][x] == 'N')
    {
        yield return (x, y + 1);
    }

    if (ValidCoordinate(map, x - 1, y) && map[y][x - 1] == 'E')
    {
        yield return (x - 1, y);
    }

    if (ValidCoordinate(map, x + 1, y) && map[y][x + 1] == 'W')
    {
        yield return (x + 1, y);
    }
}

static bool ValidCoordinate(IReadOnlyList<char[]> map, int x, int y) =>
    x > -1 && y > -1 && y < map.Count && x < map[0].Length;

static char[][] ReduceMap(IReadOnlyList<char[]> map)
{
    ((int min, int max) x, (int min, int max) y) bounds = ((int.MaxValue, int.MinValue), (int.MaxValue, int.MinValue));
    for (var y = 0; y < map.Count; ++y)
    {
        for (var x = 0; x < map[y].Length; ++x)
        {
            if (map[y][x] == '#')
            {
                bounds = (x: (Math.Min(bounds.x.min, x), Math.Max(bounds.x.max, x)),
                    y: (Math.Min(bounds.y.min, y), Math.Max(bounds.y.max, y)));
            }
        }
    }

    return map.Skip(bounds.y.min)
        .Take(bounds.y.max - bounds.y.min + 1)
        .Select(row => row[bounds.x.min..(bounds.x.max + 1)])
        .ToArray();
}