using System.Text.RegularExpressions;

var lines = File.ReadAllLines(args[0]);
var board = CreateBoard(lines[..^2]);
var instructions = Regex.Split(lines.Last(), "(L|R)");

var pos = new Point2D(lines.First().IndexOf('.'), 0);
var direction = '>';
var directionPointMap = new Dictionary<char, int> { { '>', 0 }, { '<', 2 }, { 'v', 1 }, { '^', 3 } };

var path = new Dictionary<Point2D, char>
{
    { pos, direction }
};

var wraps = CalculateWraps(board);

foreach (var step in instructions)
{
    //Print(board, path);

    if (int.TryParse(step, out var tiles))
    {
        while (tiles > 0 && TryMove(board, wraps, pos, direction, out var end, out var endDir))
        {
            (pos, direction) = (end, endDir);
            --tiles;
            path[pos] = direction;
        }
    }
    else
    {
        direction = Turn(direction, step[0]);
        path[pos] = direction;
    }
}

//Print(board, path);

var password = 1000 * (pos.Y + 1) + 4 * (pos.X + 1) + directionPointMap[direction];
Console.WriteLine(
    $"The password is 1000 * {pos.Y + 1} + 4 * {pos.X + 1} + {directionPointMap[direction]} = {password}");

static bool TryMove(CubeBoard board, Wraps wraps, Point2D pos, char direction, out Point2D end, out char endDirection)
{
    (end, endDirection) = (pos, direction);
    if (!wraps.ContainsKey((pos, direction)))
    {
        pos = direction switch
        {
            '>' => pos with { X = pos.X + 1 },
            '<' => pos with { X = pos.X - 1 },
            'v' => pos with { Y = pos.Y + 1 },
            '^' => pos with { Y = pos.Y - 1 },
            _ => pos
        };
    }
    else
    {
        (pos, direction) = wraps[(pos, direction)];
    }

    if (board.Flattened[pos.Y][pos.X] is '#' or ' ')
    {
        return false;
    }

    end = pos;
    endDirection = direction;
    return true;
}

static char Turn(char current, char direction) => current switch
{
    '>' when direction == 'R' => 'v',
    '>' when direction == 'L' => '^',
    'v' when direction == 'R' => '<',
    'v' when direction == 'L' => '>',
    '<' when direction == 'R' => '^',
    '<' when direction == 'L' => 'v',
    '^' when direction == 'R' => '>',
    '^' when direction == 'L' => '<',
    _ => throw new InvalidOperationException("Invalid turn")
};

// Pre-calculate all wraps.
static Wraps CalculateWraps(CubeBoard board)
{
    var edge = board.EdgeLength;
    var wraps = new Dictionary<(Point2D, char), (Point2D, char)>();
    for (var i = 0; i < edge; ++i)
    {
        // Sample input
        if (board.Flattened[board.FlattenedHeight - 1][board.FlattenedWidth - 1] == '.')
        {
            //   1
            // 234
            //   56

            // 1^
            // 2^
            wraps.Add((new Point2D(edge * 2 + i, 0), '^'), (new Point2D(edge - 1 - i, edge), 'v'));
            wraps.Add((new Point2D(edge - 1 - i, edge), '^'), (new Point2D(edge * 2 + i, 0), 'v'));

            // 1<
            // 3^
            wraps.Add((new Point2D(edge * 2, i), '<'), (new Point2D(edge + i, edge), 'v'));
            wraps.Add((new Point2D(edge + i, edge), '^'), (new Point2D(edge * 2, i), '>'));

            // 1>
            // 6>
            wraps.Add((new Point2D(edge * 3 - 1, i), '>'), (new Point2D(edge * 4 - 1, edge * 3 - 1 - i), '<'));
            wraps.Add((new Point2D(edge * 4 - 1, edge * 3 - 1 - i), '>'), (new Point2D(edge * 3 - 1, i), '<'));

            // 2<
            // 6v
            wraps.Add((new Point2D(0, edge + i), '<'), (new Point2D(edge * 4 - 1 - i, edge * 3 - 1), '^'));
            wraps.Add((new Point2D(edge * 4 - 1 - i, edge * 3 - 1), 'v'), (new Point2D(0, edge + i), '>'));

            // 4>
            // 6^
            wraps.Add((new Point2D(edge * 3 - 1, edge + i), '>'), (new Point2D(edge * 4 - 1 - i, edge * 2), 'v'));
            wraps.Add((new Point2D(edge * 4 - 1 - i, edge * 2), '^'), (new Point2D(edge * 3 - 1, edge + i), '<'));

            // 2v
            // 5v
            wraps.Add((new Point2D(i, edge * 2 - 1), 'v'), (new Point2D(edge * 3 - 1 - i, edge * 3 - 1), '^'));
            wraps.Add((new Point2D(edge * 3 - 1 - i, edge * 3 - 1), 'v'), (new Point2D(i, edge * 2 - 1), '^'));

            // 3v
            // 5<
            wraps.Add((new Point2D(edge + i, edge * 2 - 1), 'v'), (new Point2D(edge * 2, edge * 3 - 1 - i), '>'));
            wraps.Add((new Point2D(edge * 2, edge * 3 - 1 - i), '<'), (new Point2D(edge + i, edge * 2 - 1), '^'));
        }

        else
        {
            //  12
            //  3
            // 45
            // 6

            // 1^
            // 6<
            wraps.Add((new Point2D(edge + i, 0), '^'), (new Point2D(0, edge * 3 + i), '>'));
            wraps.Add((new Point2D(0, edge * 3 + i), '<'), (new Point2D(edge + i, 0), 'v'));

            // 2^
            // 6v
            wraps.Add((new Point2D(edge * 2 + i, 0), '^'), (new Point2D(i, edge * 4 - 1), '^'));
            wraps.Add((new Point2D(i, edge * 4 - 1), 'v'), (new Point2D(edge * 2 + i, 0), 'v'));

            // 1<
            // 4<
            wraps.Add((new Point2D(edge, i), '<'), (new Point2D(0, edge * 3 - 1 - i), '>'));
            wraps.Add((new Point2D(0, edge * 3 - 1 - i), '<'), (new Point2D(edge, i), '>'));

            // 2>
            // 5>
            wraps.Add((new Point2D(edge * 3 - 1, i), '>'), (new Point2D(edge * 2 - 1, edge * 3 - 1 - i), '<'));
            wraps.Add((new Point2D(edge * 2 - 1, edge * 3 - 1 - i), '>'), (new Point2D(edge * 3 - 1, i), '<'));

            // 3<
            // 4^
            wraps.Add((new Point2D(edge, edge + i), '<'), (new Point2D(i, edge * 2), 'v'));
            wraps.Add((new Point2D(i, edge * 2), '^'), (new Point2D(edge, edge + i), '>'));

            // 3>
            // 2v
            wraps.Add((new Point2D(edge * 2 - 1, edge + i), '>'), (new Point2D(edge * 2 + i, edge - 1), '^'));
            wraps.Add((new Point2D(edge * 2 + i, edge - 1), 'v'), (new Point2D(edge * 2 - 1, edge + i), '<'));

            // 5v
            // 6>
            wraps.Add((new Point2D(edge + i, edge * 3 - 1), 'v'), (new Point2D(edge - 1, edge * 3 + i), '<'));
            wraps.Add((new Point2D(edge - 1, edge * 3 + i), '>'), (new Point2D(edge + i, edge * 3 -1), '^'));
        }
    }

    // All of this calculates the edge connections for any edge connection where either edge
    // is "inside" the map. I couldn't figure out the ones where both edges are outside the map,
    // so that's with the hard-coding above instead.
    //var requiredPreCalcs = new HashSet<(int x, int y, char dir)>(board.EdgeLength * 14);
    //for (var y = 0; y < board.FlattenedHeight; ++y)
    //{
    //    for (var x = 0; x < board.FlattenedWidth; ++x)
    //    {
    //        if (1 < x && x < board.FlattenedWidth - 1 && !IsOnFace(x, y))
    //        {
    //            if (IsOnFace(x - 1, y))
    //            {
    //                requiredPreCalcs.Add((x, y, '>'));
    //            }
    //            else if (IsOnFace(x + 1, y))
    //            {
    //                requiredPreCalcs.Add((x, y, '<'));
    //            }
    //        }

    //        if (1 < y && y < board.FlattenedHeight - 1 && !IsOnFace(x, y))
    //        {
    //            if (IsOnFace(x, y - 1))
    //            {
    //                requiredPreCalcs.Add((x, y, 'v'));
    //            }
    //            else if (IsOnFace(x, y + 1))
    //            {
    //                requiredPreCalcs.Add((x, y, '^'));
    //            }
    //        }
    //    }
    //}

    //bool IsOnFace(int col, int row) => board.Flattened[row][col] is '.' or '#';

    //var wraps = new Dictionary<(Point2D, char), (Point2D, char)>();
    //foreach (var (col, row, direction) in requiredPreCalcs.OrderBy(c => c.x).ThenBy(c => c.y).ThenBy(c => c.dir))
    //{
    //    var pos = new Point2D(col, row);

    //    // Inner appear in this list on both ends, only calculate one of them.
    //    if (wraps.ContainsKey((pos, direction)))
    //    {
    //        continue;
    //    }

    //    var closestX = new[]
    //    {
    //        board.Flattened[pos.Y].Select((c, i) => (c, i))
    //            .First(p => p.c is '.' or '#').i,
    //        board.Flattened[pos.Y].Select((c, i) => (c, i))
    //            .Last(p => p.c is '.' or '#').i
    //    }.MinBy(x => Math.Abs(pos.X - x));
    //    var closestY = new[]
    //    {
    //        board.Flattened.Select((c, i) => (c: c[pos.X], i))
    //            .First(p => p.c is '.' or '#').i,
    //        board.Flattened.Select((c, i) => (c: c[pos.X], i))
    //            .Last(p => p.c is '.' or '#').i
    //    }.MinBy(y => Math.Abs(pos.Y - y));

    //    // This can almost definitely be simplified.
    //    if (direction is '<' or '>')
    //    {
    //        // Immediately adjacent edge
    //        if ((Math.Abs(closestY - pos.Y) - 1) / board.EdgeLength == 0)
    //        {
    //            // Figure out how far vertically I am from the nearest horizontal edge
    //            // Add that to the current horizontal value, accounting for the fact that when moving up or left,
    //            // we need to subtract, not add.
    //            var wrapDir = closestY - pos.Y > 0 ? 'v' : '^';
    //            var wrapTo = new Point2D(closestX + Math.Abs(pos.Y - closestY) * (direction == '<' ? -1 : 1), closestY);
    //            wraps.Add((pos, direction), (wrapTo, wrapDir));

    //            var otherDir = OppositeDirection(wrapDir);
    //            var otherWrapDir = OppositeDirection(direction);
    //            var otherPos = wrapTo with { Y = wrapTo.Y + (otherDir == '^' ? -1 : 1) };
    //            var otherWrapTo = pos with { X = pos.X + (direction == '<' ? 1 : -1) };
    //            wraps.Add((otherPos, otherDir), (otherWrapTo, otherWrapDir));
    //        }
    //        // Still the immediately adjacent face, but a different edge of it
    //        else
    //        {
    //            // Add face length to position.
    //            var x = pos.X + (direction is '<' ? board.EdgeLength * -1 + 1 : board.EdgeLength - 1);
    //            // Distance from nearest edge minus edge length and add it to nearest edge.
    //            var y = Math.Abs(closestY - pos.Y) - board.EdgeLength + closestY - 1;
    //            var wrapTo = new Point2D(x, y);
    //            var wrapDir = OppositeDirection(direction);
    //            wraps.Add((pos, direction), (wrapTo, wrapDir));

    //            var otherPos = wrapTo with { X = wrapTo.X + (direction == '>' ? 1 : -1) };
    //            var otherWrapTo = pos with { X = pos.X + (direction == '<' ? 1 : -1) };
    //            wraps.Add((otherPos, direction), (otherWrapTo, wrapDir));
    //        }
    //    }
    //    // direction is '^' or 'v'
    //    else
    //    {
    //        // Immediately adjacent edge
    //        if ((Math.Abs(closestX - pos.X) - 1) / board.EdgeLength == 0)
    //        {
    //            // Figure out how far vertically I am from the nearest horizontal edge
    //            // Add that to the current horizontal value, accounting for the fact that when moving up or left,
    //            // we need to subtract, not add.
    //            var wrapDir = closestX - pos.X > 0 ? '>' : '<';
    //            var wrapTo = new Point2D(closestX, closestY + Math.Abs(pos.X - closestX) * (direction == '^' ? -1 : 1));
    //            wraps.Add((pos, direction), (wrapTo, wrapDir));

    //            var otherDir = OppositeDirection(wrapDir);
    //            var otherWrapDir = OppositeDirection(direction);
    //            var otherPos = wrapTo with { X = wrapTo.X + (otherDir == '<' ? -1 : 1) };
    //            var otherWrapTo = pos with { Y = pos.Y + (direction == '^' ? 1 : -1) };
    //            _ = wraps.TryAdd((otherPos, otherDir), (otherWrapTo, otherWrapDir));
    //        }
    //        // Still immediately adjacent face, but a different edge of it
    //        else
    //        {
    //            // Distance from nearest edge minus edge length and add it to nearest edge.
    //            var x = Math.Abs(closestX - pos.X) - board.EdgeLength + closestX - 1;
    //            // Add face length to position.
    //            var y = pos.Y + (direction is '^' ? board.EdgeLength * -1 + 1 : board.EdgeLength - 1);
    //            var wrapTo = new Point2D(x, y);
    //            var wrapDir = OppositeDirection(direction);
    //            wraps.Add((pos, direction), (wrapTo, wrapDir));

    //            var otherPos = wrapTo with { Y = wrapTo.Y + (wrapDir == '^' ? 1 : -1) };
    //            var otherWrapTo = pos with { Y = pos.Y + (direction == '^' ? 1 : -1) };
    //            wraps.Add((otherPos, direction), (otherWrapTo, wrapDir));
    //        }
    //    }

    //    char OppositeDirection(char d) => d switch
    //    {
    //        '>' => '<', 'v' => '^', '<' => '>', '^' => 'v',
    //        _ => throw new ArgumentOutOfRangeException(nameof(d), d, null)
    //    };
    //}

    return new Wraps(wraps);
}

static CubeBoard CreateBoard(IReadOnlyList<string> input)
{
    var max = input.Max(x => x.Length);
    var map = new char[input.Count][];
    for (var line = 0; line < input.Count; ++line)
    {
        if (input[line].Length == max)
        {
            map[line] = input[line].ToCharArray();
        }
        else
        {
            map[line] = input[line].ToCharArray().Concat(Enumerable.Repeat(' ', max - input[line].Length)).ToArray();
        }
    }

    return new CubeBoard(map);
}

static void Print(CubeBoard board, IDictionary<Point2D, char>? path = null)
{
    for (var row = 0; row < board.FlattenedHeight; ++row)
    {
        for (var col = 0; col < board.FlattenedWidth; ++col)
        {
            var coordinate = path is null ? null : new Point2D(col, row);
            if (path?.ContainsKey(coordinate!) ?? false)
            {
                Console.Write(path[coordinate!]);
            }
            else
            {
                Console.Write(board.Flattened[row][col]);
            }
        }

        Console.Write(Environment.NewLine);
    }

    Console.Write(Environment.NewLine);
}

internal record Wraps(Dictionary<(Point2D, char), (Point2D, char)> Map)
{
    public bool ContainsKey((Point2D, char) key) => Map.ContainsKey(key);

    public (Point2D, char) this[(Point2D, char) key]
    {
        get => Map[key];
        set => Map[key] = value;
    }
}

internal record Point2D(int X, int Y);

internal record CubeBoard(char[][] Flattened)
{
    public int FlattenedHeight = Flattened.Length;
    public int FlattenedWidth => Flattened[0].Length;

    public int EdgeLength = Flattened.Length > Flattened[0].Length ? Flattened.Length / 4 : Flattened[0].Length / 4;
}