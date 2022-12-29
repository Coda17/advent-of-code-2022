using System.Collections.Immutable;
using System.Text.RegularExpressions;

var regex = new Regex("([A-Z]{2}|\\d+)");

var valves = File.ReadAllLines(args[0])
    .Select(line => regex.Matches(line))
    .Select(matches =>
        new Valve(matches[0].Value, int.Parse(matches[1].Value), matches.Skip(2).Select(m => m.Value).ToArray()))
    .ToArray();

var f = valves.ToImmutableDictionary(x => x.Name, x => x.FlowRate);
var d = FloydWarshall(valves.ToArray());

var cache = new Dictionary<string, long>();

Console.WriteLine(Search(26, valves.Where(x => x.FlowRate > 0).Select(x => x.Name).ToHashSet(), "AA", true));

// All credit to this redditor
// https://www.reddit.com/r/adventofcode/comments/zn6k1l/comment/j0fti6c/?utm_source=share&utm_medium=web2x&context=3
long Search(int t, IReadOnlyCollection<string> vs, string u = "AA", bool e = false)
{
    var key = $"{t},{u},{e},({string.Join(",", vs.OrderBy(x => x))})";
    if (cache.ContainsKey(key))
    {
        return cache[key];
    }

    var elephant = e ? Search(26, vs) : 0;
    var max = vs.Where(v => d[(u, v)] < t)
        .Aggregate(elephant,
            (i, v) => long.Max(i,
                f[v] * (t - d[(u, v)] - 1) + Search(t - d[(u, v)] - 1, vs.Except(new[] { v }).ToHashSet(), v, e)));

    cache[key] = max;
    return max;
}

static Dictionary<(string, string), int> FloydWarshall(Valve[] graph)
{
    var dist = new Dictionary<(string, string), int>();
    foreach (var u in graph)
    {
        foreach (var v in graph)
        {
            dist[(u.Name, v.Name)] = int.MaxValue;
        }
    }

    foreach (var u in graph)
    {
        foreach (var v in u.Neighbors)
        {
            // weight edge of (u, v) = 1
            dist[(u.Name, v)] = 1;
        }
    }

    foreach (var v in graph)
    {
        dist[(v.Name, v.Name)] = 0;
    }

    foreach (var k in graph)
    {
        foreach (var i in graph)
        {
            foreach (var j in graph)
            {
                if (dist[(i.Name, k.Name)] != int.MaxValue && dist[(k.Name, j.Name)] != int.MaxValue)
                {
                    dist[(i.Name, j.Name)] = Math.Min(dist[(i.Name, j.Name)],
                        dist[(i.Name, k.Name)] + dist[(k.Name, j.Name)]);
                }
            }
        }
    }

    return dist;
}

internal sealed record Valve(string Name, int FlowRate, string[] Neighbors)
{
    public override string ToString() => Name;
}

internal sealed record State(int T, string U, ImmutableSortedSet<string> Vs)
{
    public bool Equals(State? other) => other != null && T == other.T && U == other.U && Vs.SequenceEqual(other.Vs);

    public override int GetHashCode() =>
        HashCode.Combine(T, U, Vs.Aggregate(17, HashCode.Combine));
}