var elves = new List<int>();
var calories = 0;
foreach (var line in File.ReadLines(args[0]))
{
    if (int.TryParse(line, out var value))
    {
        calories += value;
    }
    else
    {
        elves.Add(calories);
        calories = 0;
    }
}

elves.Add(calories);

elves = elves.OrderByDescending(x => x).ToList();
var top3 = elves[0] + elves[1] + elves[2];
Console.WriteLine(top3);