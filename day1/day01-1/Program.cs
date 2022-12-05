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

var max = elves.Max();
Console.WriteLine(max);