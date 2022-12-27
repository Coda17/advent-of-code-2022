var droplets = File.ReadAllLines(args[0])
    .Select(x => x.Split(',').Select(int.Parse).ToArray())
    .Select(x => new Point3D(x[0], x[1], x[2]))
    .ToHashSet();

var surfaceArea = droplets.Select(droplet => new
    {
        droplet,
        sides = Neighbors(droplet)
    })
    .Select(x => x.sides.Count(droplet => !droplets.Contains(droplet)))
    .Sum();

Console.WriteLine(surfaceArea);

static IEnumerable<Point3D> Neighbors(Point3D p) => new[]
{
    p with { X = p.X - 1 },
    p with { X = p.X + 1 },
    p with { Y = p.Y - 1 },
    p with { Y = p.Y + 1 },
    p with { Z = p.Z - 1 },
    p with { Z = p.Z + 1 }
};

internal record Point3D(int X, int Y, int Z);