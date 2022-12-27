var droplets = File.ReadAllLines(args[0])
    .Select(x => x.Split(',').Select(int.Parse).ToArray())
    .Select(x => new Point3D(x[0], x[1], x[2]))
    .ToHashSet();

var (minX, maxX, minY, maxY, minZ, maxZ) = droplets.Aggregate(
    (minX: int.MaxValue, maxX: int.MinValue,
        minY: int.MaxValue, maxY: int.MinValue,
        minZ: int.MaxValue, maxZ: int.MinValue),
    (i, d) =>
        (int.Min(i.minX, d.X - 1), int.Max(i.maxX, d.X + 1),
            int.Min(i.minY, d.Y - 1), int.Max(i.maxY, d.Y + 1),
            int.Min(i.minZ, d.Z - 1), int.Max(i.maxZ, d.Z + 1)));

var boundarySize = (maxX - minX) * (maxY - minY) * (maxZ - minZ);
var visited = new HashSet<Point3D>(boundarySize);
var toVisit = new Queue<Point3D>(boundarySize);
toVisit.Enqueue(new Point3D(0, 0, 0));

while (toVisit.TryDequeue(out var droplet))
{
    if (visited.Contains(droplet))
    {
        continue;
    }

    foreach (var neighbor in Neighbors(droplet)
                 .Where(n => InBounds(n) && !visited.Contains(n) && !droplets.Contains(n)))
    {
        toVisit.Enqueue(neighbor);
    }

    visited.Add(droplet);
}

var surfaceArea = droplets.Sum(droplet => Neighbors(droplet).Count(neighbor => visited.Contains(neighbor)));

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

bool InBounds(Point3D p) => minX <= p.X && p.X <= maxX && minY <= p.Y && p.Y <= maxY && minZ <= p.Z && p.Z <= maxZ;

internal record Point3D(int X, int Y, int Z);