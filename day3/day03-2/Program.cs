var lines = File.ReadAllLines(args[0]);
var groups = lines.Chunk(3);
var totalPriority = 0;

foreach (var group in groups)
{
    var items = group[0].Intersect(group[1]).Intersect(group[2]).ToArray();
    var priority = char.IsLower(items[0]) ? items[0] - 'a' + 1 : items[0] - 'A' + 27;
    totalPriority += priority;
}

Console.WriteLine(totalPriority);