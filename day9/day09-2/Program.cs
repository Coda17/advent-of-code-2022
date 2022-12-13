using day09_2;

var rope = new Rope();
var tailVisits = new HashSet<(int, int)>();

foreach (var line in File.ReadLines(args[0]))
{
    var direction = line[0];
    var distance = int.Parse(line.Split(' ')[1]);

    for (var i = 0; i < distance; ++i)
    {
        rope.MoveHead(direction);
        tailVisits.Add((rope.Knots[9].Item1, rope.Knots[9].Item2));
    }
}

Console.WriteLine(tailVisits.Count);