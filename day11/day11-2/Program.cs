using System.Linq.Expressions;
using day11_2;

var lines = File.ReadAllLines(args[0]);
var monkeys = new List<Monkey>((lines.Length + 1)/ 7);

var i = 0;
while (i < lines.Length)
{
    var items = lines[i + 1].Replace("  Starting items: ", string.Empty).Split(", ").Select(long.Parse);
    var operations = lines[i + 2].Replace("  Operation: new = ", string.Empty).Split(" ");

    var old = Expression.Parameter(typeof(long), "old");
    Expression left = operations[0] == "old" ? old : Expression.Constant(long.Parse(operations[0]), typeof(long));
    Expression right = operations[2] == "old" ? old : Expression.Constant(long.Parse(operations[2]), typeof(long));

    var operation = Expression
        .Lambda<Func<long, long>>(operations[1] == "+" ? Expression.Add(left, right) : Expression.Multiply(left, right), old)
        .Compile();

    var modulo = long.Parse(lines[i + 3].Replace("  Test: divisible by ", string.Empty));
    var trueMonkey = int.Parse(lines[i + 4].Replace("If true: throw to monkey ", string.Empty));
    var falseMonkey = int.Parse(lines[i + 5].Replace("If false: throw to monkey ", string.Empty));

    var monkey = new Monkey(new LinkedList<long>(items), operation, modulo, trueMonkey, falseMonkey);
    monkeys.Add(monkey);

    i += 7;
}

var factor = monkeys.Select(x => x.Modulo).Aggregate(1L, (l, l1) => l * l1);
for (var round = 1; round < 10001; ++round)
{
    foreach (var monkey in monkeys)
    {
        while (monkey.Items.Any())
        {
            var item = monkey.Items.First();
            monkey.Items.RemoveFirst();

            var newItem = monkey.Operation(item) % factor;

            var newMonkey = newItem % monkey.Modulo == 0 ? monkey.TrueMonkey : monkey.FalseMonkey;
            monkeys[newMonkey].Items.AddLast(newItem);

            ++monkey.Inspections;
        }
    }
}

var inspections = monkeys.OrderByDescending(x => x.Inspections).Select(x => x.Inspections).ToArray();
var monkeyBusiness = inspections[0] * inspections[1];

Console.WriteLine(monkeyBusiness);