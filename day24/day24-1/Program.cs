using System.Collections;
using System.Numerics;

var map = new Map(File.ReadLines(args[0])
    .Select(x => x.ToCharArray())
    .ToArray());

var root = (Array.IndexOf(map[0], Space.None), 0);
var goal = (Array.IndexOf(map[map.Height - 1], Space.None), map.Height - 1);

var states = CalculateMapStates(map);
var minutes = Bfs(states, root, goal);
Console.WriteLine(minutes);

static int Bfs(IReadOnlyList<Map> states, (int x, int y) root, (int x, int y) goal)
{
    var q = new Queue<(int x, int y, int s, int d)>();
    q.Enqueue((root.x, root.y, 0, 0));

    var explored = new HashSet<(int x, int y, int s)>
    {
        (root.x, root.y, 0)
    };

    while (q.TryDequeue(out var v))
    {
        if (v.x == goal.x && v.y == goal.y)
        {
            return v.d;
        }

        // Make sure to use the NEXT state since everything moves simultaneously.
        var next = (v.s + 1) % states.Count;
        foreach (var w in Neighbors(states[next], (v.x, v.y)))
        {
            if (explored.Contains((w.x, w.y, next)))
            {
                continue;
            }

            explored.Add((w.x, w.y, next));
            q.Enqueue((w.x, w.y, next, v.d + 1));
        }
    }

    return -1;

    IEnumerable<(int x, int y)> Neighbors(Map state, (int x, int y) v)
    {
        if (state[v.y][v.x - 1] == Space.None)
        {
            yield return (v.x - 1, v.y);
        }

        if (state[v.y][v.x + 1] == Space.None)
        {
            yield return (v.x + 1, v.y);
        }

        // Don't accidentally leave map through root.
        if (v.y != 0 && state[v.y - 1][v.x] == Space.None)
        {
            yield return (v.x, v.y - 1);
        }

        if (state[v.y + 1][v.x] == Space.None)
        {
            yield return (v.x, v.y + 1);
        }

        // Wait.
        if (state[v.y][v.x] == Space.None)
        {
            yield return (v.x, v.y);
        }
    }
}

static Map[] CalculateMapStates(Map map)
{
    var (width, height) = (map.Width, map.Height);
    var lcm = Lcm(width - 2, height - 2);

    var states = new Map[lcm];
    var prev = (Map) map.Clone();
    for (var i = 0; i < lcm; ++i)
    {
        states[i] = (Map) prev.Clone();

        for (var y = 1; y < height - 1; ++y)
        {
            for (var x = 1; x < width - 1; ++x)
            {
                var space = IncomingBlizzards(states[i], x, y);
                prev[y][x] = space;
            }
        }
    }

    return states;

    Space IncomingBlizzards(Map map, int x, int y)
    {
        var left = x - 1 == 0 ? width - 2 : x - 1;
        var right = x + 1 == width - 1 ? 1 : x + 1;
        var up = y - 1 == 0 ? height - 2 : y - 1;
        var down = y + 1 == height - 1 ? 1 : y + 1;

        var leftIsBlizzard = map[y][left].HasFlag(Space.Right) ? Space.Right : Space.None;
        var rightIsBlizzard = map[y][right].HasFlag(Space.Left) ? Space.Left : Space.None;
        var upIsBlizzard = map[up][x].HasFlag(Space.Down) ? Space.Down : Space.None;
        var downIsBlizzard = map[down][x].HasFlag(Space.Up) ? Space.Up: Space.None;

        return leftIsBlizzard | rightIsBlizzard | upIsBlizzard | downIsBlizzard;
    }
}

static int Gcd(int a, int b)
{
    while (b != 0)
    {
        var c = b;
        b = a % b;
        a = c;
    }

    return a;
}

static int Lcm(int a, int b) => a / Gcd(a, b) * b;

static void Print(Map map, (int x, int y)? current = default)
{
    for (var y = 0; y < map.Height; ++y)
    {
        for (var x = 0; x < map.Width; ++x)
        {
            var bits = BitOperations.PopCount((uint) map[y][x]);
            if (current.HasValue && x == current.Value.x && y == current.Value.y)
            {
                Console.Write('E');
            }
            else if (bits > 1)
            {
                Console.Write(bits);
            }
            else
            {
                Console.Write(map[y][x] switch
                {
                    Space.None => '.',
                    Space.Up => '^',
                    Space.Right => '>',
                    Space.Down => 'v',
                    Space.Left => '<',
                    Space.Wall => '#',
                    _ => throw new InvalidOperationException()
                });
            }
        }

        Console.Write(Environment.NewLine);
    }
}

internal sealed class Map : IEnumerable<Space[]>, ICloneable
{
    public int Width => _spaces[0].Length;
    public int Height => _spaces.Length;
    public Space[] this[int key] => _spaces[key];

    private readonly Space[][] _spaces;

    public Map(IEnumerable<char[]> spaces)
    {
        _spaces = spaces.Select(y => y.Select(x => x switch
        {
            '.' => Space.None,
            '#' => Space.Wall,
            '^' => Space.Up,
            '>' => Space.Right,
            'v' => Space.Down,
            '<' => Space.Left,
            _ => throw new InvalidOperationException()
        }).ToArray()).ToArray();
    }

    private Map(Map map)
    {
        _spaces = new Space[map.Height][];
        for (var y = 0; y < map.Height; ++y)
        {
            _spaces[y] = new Space[map.Width];
            for (var x = 0; x < map.Width; ++x)
            {
                _spaces[y][x] = map[y][x];
            }
        }
    }

    public object Clone() => new Map(this);

    public IEnumerator<Space[]> GetEnumerator() => (IEnumerator<Space[]>) _spaces.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => _spaces.GetEnumerator();
}

[Flags]
internal enum Space : uint
{
    None  = 0b00000,
    Up    = 0b00001,
    Right = 0b00010,
    Down  = 0b00100,
    Left  = 0b01000,
    Wall  = 0b10000
}