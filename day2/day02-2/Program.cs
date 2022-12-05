IDictionary<(char, char), int> pointMap = new Dictionary<(char, char), int>
{
    { ('A', 'X'), 3 },
    { ('A', 'Y'), 4 },
    { ('A', 'Z'), 8 },
    { ('B', 'X'), 1 },
    { ('B', 'Y'), 5 },
    { ('B', 'Z'), 9 },
    { ('C', 'X'), 2 },
    { ('C', 'Y'), 6 },
    { ('C', 'Z'), 7 },
};

var points = 0;
foreach (var line in File.ReadLines(args[0]))
{
    var round = line.Split(' ').Select(x => x[0]).ToArray();
    var (opponent, me) = (round[0], round[1]);
    points += pointMap[(opponent, me)];
}

Console.WriteLine(points);