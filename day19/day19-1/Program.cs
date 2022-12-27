using System.Text.RegularExpressions;

var regex = new Regex("(\\d+)");
var blueprints = File.ReadLines(args[0])
    .Select(l => regex.Matches(l).Select(m => long.Parse(m.Value)).ToArray()).Select(x =>
        new Blueprint(x[0],
            new[]
            {
                (new Materials(Ore: x[5], Obsidian: x[6]), new Materials(Geodes: 1)),
                (new Materials(Ore: x[3], Clay: x[4]), new Materials(Obsidian: 1)),
                (new Materials(Ore: x[2]), new Materials(Clay: 1)),
                (new Materials(Ore: x[1]), new Materials(Ore: 1)),
                // The not making a robot case
                (new Materials(), new Materials())
            }));

Console.WriteLine(blueprints.Sum(blueprint => Max(blueprint, 24) * blueprint.N));

static long Max(Blueprint blueprint, int minutes)
{
    var prunes = new List<State>
    {
        new(new Materials(), new Materials(Ore: 1))
    };

    foreach (var _ in Enumerable.Range(0, minutes))
    {
        var branches = Branch(blueprint, prunes);
        prunes = branches
            .OrderByDescending(x => (x.Have + x.Produce).Geodes)
            .ThenByDescending(x => (x.Have + x.Produce).Obsidian)
            .ThenByDescending(x => (x.Have + x.Produce).Clay)
            .ThenByDescending(x => (x.Have + x.Produce).Ore)
            .ThenByDescending(x => x.Produce.Geodes)
            .ThenByDescending(x => x.Produce.Obsidian)
            .ThenByDescending(x => x.Produce.Clay)
            .ThenByDescending(x => x.Produce.Ore)
            .Take(1000)
            .ToList();
    }

    return prunes.Select(x => x.Have.Geodes).Max();
}

static IEnumerable<State> Branch(Blueprint blueprint, List<State> states)
{
    foreach (var state in states)
    {
        foreach (var (need, build) in blueprint.Next)
        {
            if (state.Have.Contains(need))
            {
                yield return new State(state.Have + state.Produce - need,
                    state.Produce + build);
            }
        }
    }
}

internal sealed record Blueprint(long N, (Materials, Materials)[] Next);

internal sealed record Materials(long Geodes = 0, long Obsidian = 0, long Clay = 0, long Ore = 0)
{
    public bool Contains(Materials other) =>
        Geodes >= other.Geodes && Obsidian >= other.Obsidian && Clay >= other.Clay && Ore >= other.Ore;

    public static Materials operator +(Materials left, Materials right) => new(
        left.Geodes + right.Geodes,
        left.Obsidian + right.Obsidian,
        left.Clay + right.Clay,
        left.Ore + right.Ore);

    public static Materials operator -(Materials left, Materials right) => new(
        left.Geodes - right.Geodes,
        left.Obsidian - right.Obsidian,
        left.Clay - right.Clay,
        left.Ore - right.Ore);
}

internal sealed record State(Materials Have, Materials Produce);