IDictionary<(char, char), int> pointMap = new Dictionary<(char, char), int>
{
    { ('A', 'X'), 4 },
    { ('A', 'Y'), 8 },
    { ('A', 'Z'), 3 },
    { ('B', 'X'), 1 },
    { ('B', 'Y'), 5 },
    { ('B', 'Z'), 9 },
    { ('C', 'X'), 7 },
    { ('C', 'Y'), 2 },
    { ('C', 'Z'), 6 },
};

var points = 0;
foreach (var line in File.ReadLines(args[0]))
{
    var round = line.Split(' ').Select(x => x[0]).ToArray();
    var (opponent, me) = (round[0], round[1]);
    points += pointMap[(opponent, me)];
}

Console.WriteLine(points);