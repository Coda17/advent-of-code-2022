var totalPriority = 0;
foreach (var line in File.ReadLines(args[0]))
{
    var compartments = line.Chunk(line.Length / 2).Select(x => new string(x.Distinct().ToArray())).ToArray();
    var items = compartments[0].Intersect(compartments[1]).ToArray();
    var priority = char.IsLower(items[0]) ? items[0] - 'a' + 1 : items[0] - 'A' + 27;
    totalPriority += priority;
}

Console.WriteLine(totalPriority);