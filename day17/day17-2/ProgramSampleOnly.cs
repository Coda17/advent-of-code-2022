//var rocks = new[]
//{
//    // ####
//    new Shape(new[] { 2L, 3L, 4L, 5L }),

//    // .#.
//    // ###
//    // .#.
//    new Shape(new[] { 3L, 9L, 10L, 11L, 17L }),

//    // ..#
//    // ..#
//    // ###
//    new Shape(new[] { 2L, 3L, 4L, 11L, 18L }),

//    // #
//    // #
//    // #
//    // #
//    new Shape(new[] { 2L, 9L, 16L, 23L }),

//    // ##
//    // ##
//    new Shape(new[] { 2L, 3L, 9L, 10L })
//};

//const int columns = 7;
//var jet = File.ReadLines(args[0]).First();
//var jetIndex = 0;
//var pile = new Shape(Array.Empty<long>());

//// Totals before cycle detected.
//var beforeHeight = 0L;
//var beforeRocks = 0L;

//// Totals during cycle.
//var cycleHeight = 0L;
//var cycleRocks = 0L;

//long? jetIndexOfCycleStart = null;
//var jetIndices = new HashSet<int>();

////const long totalRocks = 2022L;
//const long totalRocks = 1000000000000L;
//var rockIndex = 0;
//var targetRockCount = totalRocks;
//for (var rockCount = 0L; rockCount < targetRockCount; ++rockCount)
//{
//    var pileHeight = CalcPileHeight(pile);
//    var rock = new Shape(rocks[rockIndex].Coords.Select(x => x + (pileHeight + 3L) * columns).ToArray());

//    Draw(pile, rock);

//    var jetReset = false;
//    while (true)
//    {
//        if (TryMove(pile, rock, jet[jetIndex], out var pushed))
//        {
//            rock = pushed;
//        }

//        jetIndex = (jetIndex + 1) % jet.Length;
//        if (jetIndex == 0)
//        {
//            jetReset = true;
//        }

//        Draw(pile, rock);

//        if (TryMove(pile, rock, 'v', out var fallen))
//        {
//            rock = fallen;
//        }
//        else
//        {
//            break;
//        }

//        Draw(pile, rock);
//    }

//    Draw(pile, rock);

//    pile = new Shape(pile.Coords.Union(rock.Coords).ToArray());
//    pileHeight = CalcPileHeight(pile);

//    rockIndex = (rockIndex + 1) % rocks.Length;

//    Draw(pile);

//    if (jetReset && rockIndex == 0)
//    {
//        if (jetIndexOfCycleStart is null)
//        {
//            if (jetIndices.Contains(jetIndex))
//            {
//                beforeHeight = pileHeight;
//                beforeRocks = rockCount;
//                jetIndexOfCycleStart = jetIndex;
//            }
//            else
//            {
//                jetIndices.Add(jetIndex);
//            }
//        }
//        else if (jetIndex == jetIndexOfCycleStart)
//        {
//            cycleHeight = pileHeight - beforeHeight;
//            cycleRocks = rockCount - beforeRocks;

//            targetRockCount = rockCount + (totalRocks - beforeRocks) % cycleRocks;
//        }
//    }

//    if (rockCount > 5000L)
//    {
//        throw new Exception();
//    }
//}

//var afterHeight = CalcPileHeight(pile) - beforeHeight - cycleHeight;

//var cycleCount = cycleRocks > 0 ? (totalRocks - beforeRocks) / cycleRocks : 0;
//var height = beforeHeight + cycleCount * cycleHeight + afterHeight;

//Console.WriteLine(
//    $"Cycle started at rock {beforeRocks} with a cycle of {cycleRocks} repeated {cycleCount} times and {totalRocks - beforeRocks - cycleRocks * cycleCount} rocks at the end.");
//Console.WriteLine($"Total height {height}");
//Console.WriteLine($"({beforeHeight} + {cycleCount} * {cycleHeight} + {afterHeight}");

//static long CalcPileHeight(Shape pile) => pile.Max.HasValue ? pile.Max.Value / columns + 1L : 0L;

//static bool TryMove(Shape chamber, Shape rock, char direction, out Shape moved)
//{
//    var movement = direction switch
//    {
//        '<' => 1,
//        '>' => -1,
//        'v' => columns,
//        _ => throw new ArgumentOutOfRangeException(nameof(direction))
//    };

//    moved = new Shape(rock.Coords.Select(x => x - movement).ToArray());
//    switch (direction)
//    {
//        case 'v' when moved.Coords.Any(x => x < 0):
//        case '<' when moved.Coords.Any(x => x % columns == 6):
//        case '>' when moved.Coords.Any(x => x % columns == 0):
//            return false;
//        default:
//            return !moved.Intersects(chamber);
//    }
//}

//static void Draw(Shape pile, Shape? rock = null)
//{
//    //DrawChamber(pile, rock);
//}

//static Shape Prune(Shape shape, int rows)
//{
//    var height = shape.Max.HasValue? shape.Max.Value / columns + 1 : 0;
//    var rowsToCut = height - rows;
//    return rowsToCut <= 0
//        ? shape
//        : new Shape(shape.Coords.Where(x => x / columns < rowsToCut).Select(x => x - rowsToCut * columns).ToArray());
//}

//static void DrawChamber(Shape pile, Shape? rock)
//{
//    var maxHeight = Math.Max(pile.Max.HasValue ? pile.Max.Value / columns + 1 : 0, rock?.Coords.Max() / columns ?? 0);
//    for (var y = maxHeight; y >= 0; --y)
//    {
//        Console.Write('|');
//        for (var x = 0; x < columns; ++x)
//        {
//            if (pile.Coords.Contains(y * columns + x))
//            {
//                Console.Write('#');
//            }
//            else if (rock is not null && rock.Coords.Contains(y * columns + x))
//            {
//                Console.Write('@');
//            }
//            else
//            {
//                Console.Write('.');
//            }
//        }

//        Console.Write($"|{Environment.NewLine}");
//    }

//    Console.WriteLine("+-------+");
//    Console.Write(Environment.NewLine);
//}

//public sealed record Shape(long[] Coords)
//{
//    public long? Max => Coords.Length == 0 ? null : Coords.Max();

//    public bool Intersects(Shape other) => Coords.Any(x => other.Coords.Contains(x));

//    public bool Equals(Shape? other) => other != null && Coords.SequenceEqual(other.Coords);
//    public override int GetHashCode() => Coords.Aggregate(17, (a, x) => HashCode.Combine(a, x.GetHashCode()));
//}