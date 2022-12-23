var rocks = new[]
{
    // ..####.
    new Shape(new[] { 2, 3, 4, 5 }),

    // ...#...
    // ..###..
    // ...#...
    new Shape(new[] { 3, 9, 10, 11, 17 }),

    // ....#..
    // ....#..
    // ..###..
    new Shape(new[] { 2, 3, 4, 11, 18 }),

    // ..#....
    // ..#....
    // ..#....
    // ..#....
    new Shape(new[] { 2, 9, 16, 23 }),

    // ..##...
    // ..##...
    new Shape(new[] { 2, 3, 9, 10 })
};

const int columns = 7;
var jet = File.ReadLines(args[0]).First();
var jetIndex = 0;
var pile = new Shape(Array.Empty<int>());

const long totalCycles = 2022L;
for (var i = 0L; i < totalCycles; ++i)
{
    var pileHeight = pile.Max.HasValue ? pile.Max.Value / columns + 1 : 0;
    var rock = new Shape(rocks[i % rocks.Length].Coords.Select(x => x + (pileHeight + 3) * columns).ToArray());

    Draw(pile, rock);

    while (true)
    {
        if (TryMove(pile, rock, jet[jetIndex], out var pushed))
        {
            rock = pushed;
        }

        jetIndex = (jetIndex + 1) % jet.Length;

        Draw(pile, rock);

        if (TryMove(pile, rock, 'v', out var fallen))
        {
            rock = fallen;
        }
        else
        {
            break;
        }

        Draw(pile, rock);
    }

    Draw(pile, rock);

    pile = new Shape(Coords: pile.Coords.Union(rock.Coords).ToArray());

    Draw(pile);
}

Console.WriteLine(pile.Coords.Max() / columns + 1);

static bool TryMove(Shape pile, Shape rock, char direction, out Shape moved)
{
    var movement = direction switch
    {
        '<' => 1,
        '>' => -1,
        'v' => columns,
        _ => throw new ArgumentOutOfRangeException(nameof(direction))
    };

    moved = new Shape(rock.Coords.Select(x => x - movement).ToArray());
    switch (direction)
    {
        case 'v' when moved.Coords.Any(x => x < 0):
        case '<' when moved.Coords.Any(x => x % columns == 6):
        case '>' when moved.Coords.Any(x => x % columns == 0):
            return false;
        default:
            return !moved.Intersects(pile);
    }
}

static void Draw(Shape pile, Shape? rock = null)
{
    //DrawChamber(pile, rock);
}

static void DrawChamber(Shape pile, Shape? rock)
{
    var maxHeight = Math.Max(pile.Max.HasValue ? pile.Max.Value / columns + 1 : 0, rock?.Coords.Max() / columns ?? 0);
    for (var y = maxHeight; y >= 0; --y)
    {
        Console.Write('|');
        for (var x = 0; x < columns; ++x)
        {
            if (pile.Coords.Contains(y * columns + x))
            {
                Console.Write('#');
            }
            else if (rock is not null && rock.Coords.Contains(y * columns + x))
            {
                Console.Write('@');
            }
            else
            {
                Console.Write('.');
            }
        }

        Console.Write($"|{Environment.NewLine}");
    }

    Console.WriteLine("+-------+");
    Console.Write(Environment.NewLine);
}

public sealed record Shape(int[] Coords)
{
    public int? Max => Coords.Length == 0 ? null : Coords.Max();

    public bool Intersects(Shape other) => Coords.Any(x => other.Coords.Contains(x));
}