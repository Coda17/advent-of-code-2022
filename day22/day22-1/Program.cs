using System.Text.RegularExpressions;

var lines = File.ReadAllLines(args[0]);
var board = NormalizeBoard(lines[..^2]);
var instructions = Regex.Split(lines.Last(), "(L|R)");

var pos = new Point2D(lines.First().IndexOf('.'), 0);
var direction = '>';
var directionPointMap = new Dictionary<char, int> { { '>', 0 }, { '<', 2 }, { 'v', 1 }, { '^', 3 } };

foreach (var step in instructions)
{
    if (int.TryParse(step, out var tiles))
    {
        while (tiles > 0 && TryMove(board, pos, direction, out var end))
        {
            pos = end;
            --tiles;
        }
    }
    else
    {
        direction = Turn(direction, step[0]);
    }
}

var password = 1000 * (pos.Y + 1) + 4 * (pos.X + 1) + directionPointMap[direction];
Console.WriteLine(
    $"The password is 1000 * {pos.Y + 1} + 4 * {pos.X + 1} + {directionPointMap[direction]} = {password}");

static char[][] NormalizeBoard(IReadOnlyList<string> input)
{
    var max = input.Max(x => x.Length);
    var board = new char[input.Count][];
    for (var line = 0; line < input.Count; ++line)
    {
        if (input[line].Length == max)
        {
            board[line] = input[line].ToCharArray();
        }
        else
        {
            board[line] = input[line].ToCharArray().Concat(Enumerable.Repeat(' ', max - input[line].Length)).ToArray();
        }
    }

    return board;
}

static bool TryMove(IReadOnlyList<char[]> board, Point2D pos, char direction, out Point2D end)
{
    end = pos;
    pos = Move(board, pos, direction);

    if (board[pos.Y][pos.X] == ' ')
    {
        pos = Wrap(board, pos, direction);
    }

    if (board[pos.Y][pos.X] == '#' || board[pos.Y][pos.X] == ' ')
    {
        return false;
    }

    end = pos;
    return true;
}

static Point2D Move(IReadOnlyList<char[]> board, Point2D pos, char direction) => direction switch
{
    '>' => pos with { X = (pos.X + 1 + board[0].Length) % board[0].Length },
    '<' => pos with { X = (pos.X - 1 + board[0].Length) % board[0].Length },
    'v' => pos with { Y = (pos.Y + 1 + board.Count) % board.Count },
    '^' => pos with { Y = (pos.Y - 1 + board.Count) % board.Count },
    _ => pos
};

static Point2D Wrap(IReadOnlyList<char[]> board, Point2D pos, char direction)
{
    var moveable = new[] { '.', '#' };
    return direction switch
    {
        '>' => pos with { X = board[pos.Y].Select((c, i) => (c, i)).First(p => p.c is '.' or '#').i },
        '<' => pos with { X = board[pos.Y].Select((c, i) => (c, i)).Last(p => p.c is '.' or '#').i },
        '^' => pos with { Y = board.Select((row, i) => (row, i)).Last(r => moveable.Contains(r.row[pos.X])).i },
        'v' => pos with { Y = board.Select((row, i) => (row, i)).First(r => moveable.Contains(r.row[pos.X])).i },
        _ => throw new InvalidOperationException("Invalid direction")
    };
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

internal record Point2D(int X, int Y);