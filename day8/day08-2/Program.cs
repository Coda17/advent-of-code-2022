var map = File.ReadAllLines(args[0]);

var size = map.Length;
var best = 0;

for (var row = 1; row < size - 1; ++row)
{
    for (var col = 1; col < size - 1; ++col)
    {
        var tree = map[row][col];
        var left = CalculateDirectionalScore(map[row][..^(size - col)].Reverse(), tree);
        var right = CalculateDirectionalScore(map[row][(col + 1)..], tree);

        var vert = map.Select(x => x[col]).ToArray();
        var top = CalculateDirectionalScore(vert[..^(size - row)].Reverse(), tree);
        var bot = CalculateDirectionalScore(vert[(row + 1)..], tree);

        var score = left * right * top * bot;

        if (score > best)
        {
            best = score;
        }
    }
}

Console.WriteLine(best);

static int CalculateDirectionalScore(IEnumerable<char> trees, char tree)
{
    var directionalScore = 0;
    foreach (var t in trees)
    {
        ++directionalScore;
        if (t >= tree)
        {
            break;
        }
    }

    return directionalScore;
}