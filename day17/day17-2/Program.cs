var rocks = new[]
{
    // ####
    new Shape(new[] { 2L, 3L, 4L, 5L }),

    // .#.
    // ###
    // .#.
    new Shape(new[] { 3L, 9L, 10L, 11L, 17L }),

    // ..#
    // ..#
    // ###
    new Shape(new[] { 2L, 3L, 4L, 11L, 18L }),

    // #
    // #
    // #
    // #
    new Shape(new[] { 2L, 9L, 16L, 23L }),

    // ##
    // ##
    new Shape(new[] { 2L, 3L, 9L, 10L })
};

const int columns = 7;
var jet = File.ReadLines(args[0]).First();
var jetIndex = 0;
var pile = new Shape(Array.Empty<long>());

long? cycleStart = null;
var beforeCycleHeight = 0L;

var potentialCycleStarts = new Dictionary<(int jetIndex, int rockIndex, Shape pile), (long rock, long height)>();

const long totalRocks = 1000000000000L;
var rockIndex = 0;
var totalHeight = 0L;
for (var rockCount = 0L; rockCount < totalRocks; ++rockCount)
{
    var startHeight = CalcPileHeight(pile);
    var rock = new Shape(rocks[rockIndex].Coords.Select(x => x + (startHeight + 3L) * columns).ToArray());

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

    pile = new Shape(pile.Coords.Union(rock.Coords).ToArray());
    var newHeight = CalcPileHeight(pile);
    totalHeight += newHeight - startHeight;

    if (TryPrune(pile, rock, out var pruned))
    {
        pile = pruned;

        // Cycle detected.
        if (potentialCycleStarts.ContainsKey((jetIndex, rockIndex, pile)))
        {
            (cycleStart, beforeCycleHeight) = potentialCycleStarts[(jetIndex, rockIndex, pile)];
        }
        // This is a potential cycle start.
        else
        {
            potentialCycleStarts.Add((jetIndex, rockIndex, pile), (rock: rockCount, height: totalHeight));
        }
    }

    Draw(pile);

    // Full cycle finished.
    if (cycleStart.HasValue)
    {
        var rocksPerCycle = rockCount - cycleStart.Value;
        var cycleHeight = totalHeight - beforeCycleHeight;
        var rocksLeft = totalRocks - rockCount;
        var cyclesLeft = rocksLeft / rocksPerCycle;

        totalHeight += cyclesLeft * cycleHeight;
        rockCount += cyclesLeft * rocksPerCycle;

        // We won't find another cycle, don't do this block again.
        cycleStart = null;
    }

    rockIndex = (rockIndex + 1) % rocks.Length;
}

Console.WriteLine(totalHeight);

static long CalcPileHeight(Shape pile) => pile.Max.HasValue ? pile.Max.Value / columns + 1L : 0L;

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

static bool TryPrune(Shape pile, Shape rock, out Shape pruned)
{
    // actually one above bottom rock row because of our odd input
    var bottomRockRow = rock.Coords.Min() / columns + 1;
    if (bottomRockRow < 2)
    {
        pruned = pile;
        return false;
    }

    var rowStart = bottomRockRow * columns;
    var pileRow = pile.Coords.Where(x => rowStart <= x && x <= rowStart + 2 * columns)
        .Select(x => x % columns)
        .Distinct()
        .OrderBy(x => x)
        .ToArray();

    var passable = !pileRow
        .SequenceEqual(Enumerable.Range(0, columns).Select(x => (long) x));

    if (passable)
    {
        pruned = pile;
        return false;
    }

    pruned = new Shape(pile.Coords.Where(x => x >= rowStart).Select(x => x - rowStart).ToArray());
    return true;
}

static void Draw(Shape pile, Shape? rock = null)
{
    //DrawChamber(pile, rock);

    //if (rock is null)
    //{
    //    DrawChamber(pile, null);
    //}
}

static void DrawChamber(Shape pile, Shape? rock)
{
    var maxHeight = Math.Max(pile.Max.HasValue ? pile.Max.Value / columns + 1 : 0, rock?.Coords.Max() / columns ?? 0);
    for (var y = maxHeight; y >= 0; --y)
    {
        Console.Write($"{y:d4} |");
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

    Console.WriteLine("     +-------+");
    Console.Write(Environment.NewLine);
}

public sealed record Shape(long[] Coords)
{
    public long? Max => Coords.Length == 0 ? null : Coords.Max();

    public bool Intersects(Shape other) => Coords.Any(x => other.Coords.Contains(x));

    public bool Equals(Shape? other) => other != null && Coords.SequenceEqual(other.Coords);
    public override int GetHashCode() => Coords.Aggregate(17, (a, x) => HashCode.Combine(a, x.GetHashCode()));
}