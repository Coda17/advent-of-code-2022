using System.Linq.Expressions;
using day11_1;

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

    var divisibleBy = Expression.Constant(long.Parse(lines[i + 3].Replace("  Test: divisible by ", string.Empty)),
        typeof(long));
    var ifTrue = Expression.Constant(int.Parse(lines[i + 4].Replace("If true: throw to monkey ", string.Empty)), typeof(int));
    var ifFalse = Expression.Constant(int.Parse(lines[i + 5].Replace("If false: throw to monkey ", string.Empty)), typeof(int));
    var param = Expression.Parameter(typeof(long));
    var test = Expression
        .Lambda<Func<long, int>>(
            Expression.Condition(
                Expression.Equal(Expression.Modulo(param, divisibleBy), Expression.Constant(0L, typeof(long))), ifTrue,
                ifFalse), param).Compile();

    var monkey = new Monkey(new LinkedList<long>(items), operation, test);
    monkeys.Add(monkey);

    i += 7;
}

for (var round = 1; round < 21; ++round)
{
    foreach (var monkey in monkeys)
    {
        while (monkey.Items.Any())
        {
            var item = monkey.Items.First();
            monkey.Items.RemoveFirst();

            var newItem = monkey.Operation(item) / 3;
            var newMonkey = monkey.Test(newItem);
            monkeys[newMonkey].Items.AddLast(newItem);

            ++monkey.Inspections;
        }
    }
}

var inspections = monkeys.OrderByDescending(x => x.Inspections).Select(x => x.Inspections).ToArray();
var monkeyBusiness = inspections[0] * inspections[1];

Console.WriteLine(monkeyBusiness);