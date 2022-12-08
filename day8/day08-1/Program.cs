var map = File.ReadAllLines(args[0]);

var size = map.Length;
var visible = size * 2 + (size * 2 - 4);

for (var row = 1; row < size - 1; ++row)
{
    for (var col = 1; col < size - 1; ++col)
    {
        var tree = map[row][col];
        var left = map[row][(col + 1)..].All(x => x < tree);
        var right = map[row][..^(size - col)].All(x => x < tree);

        var vert = map.Select(x => x[col]).ToArray();
        var top = vert[(row + 1)..].All(x => x < tree);
        var bot = vert[..^(size - row)].All(x => x < tree);

        visible += left || right || top || bot ? 1 : 0;
    }
}

Console.WriteLine(visible);