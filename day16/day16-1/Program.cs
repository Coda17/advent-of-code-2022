using System.Collections.Immutable;
using System.Text.RegularExpressions;

var regex = new Regex("([A-Z]{2}|\\d+)");

var valves = File.ReadAllLines(args[0])
    .Select(line => regex.Matches(line))
    .Select(matches =>
        new Valve(matches[0].Value, int.Parse(matches[1].Value), matches.Skip(2).Select(m => m.Value).ToArray()))
    .OrderByDescending(x => x.FlowRate)
    .ToArray();

var f = valves.ToImmutableDictionary(x => x.Name, x => x.FlowRate);
var d = FloydWarshall(valves.ToArray());

var cache = new Dictionary<State, long>();

Console.WriteLine(Max(new State(30, "AA", valves.Where(x => x.FlowRate > 0).Select(x => x.Name).ToImmutableSortedSet())));

// All credit to this redditor
// https://www.reddit.com/r/adventofcode/comments/zn6k1l/comment/j0fti6c/?utm_source=share&utm_medium=web2x&context=3
long Max(State s)
{
    if (cache.ContainsKey(s))
    {
        return cache[s];
    }

    var max = s.Vs.Where(v => d[(s.U, v)] < s.T).Aggregate(0L,
        (i, v) => long.Max(i,
            f[v] * (s.T - d[(s.U, v)] - 1) +
            Max(new State(s.T - d[(s.U, v)] - 1, v, s.Vs.Where(x => x != v).ToImmutableSortedSet()))));

    cache[s] = max;
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