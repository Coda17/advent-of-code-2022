using System.Linq.Expressions;
using System.Text.RegularExpressions;

var regex = new Regex("(?'name'\\w+): (?'constant'\\d+)?(?'operation'(?'left'\\S+)\\s?(?'operator'[+\\-*\\/])\\s?(?'right'\\S+))?");
var matches = File.ReadAllLines(args[0]).Select(x => regex.Match(x)).ToArray();
var expressions = new Dictionary<string, Expression>(matches.Length);

foreach (var match in matches.Where(x => x.Groups["constant"].Captures.Any()))
{
    var constant = long.Parse(match.Groups["constant"].Value);
    var expression = Expression.Constant(constant, typeof(long));
    expressions.Add(match.Groups["name"].Value, expression);
}

while (expressions.Count != matches.Length)
{
    foreach (var match in matches.Where(x => !expressions.ContainsKey(x.Groups["name"].Value)))
    {
        var left = match.Groups["left"].Value;
        var right = match.Groups["right"].Value;

        var leftExpression = expressions.ContainsKey(left) ? expressions[left] : null;
        var rightExpression = expressions.ContainsKey(right) ? expressions[right] : null;

        if (leftExpression is null || rightExpression is null)
        {
            continue;
        }

        var name = match.Groups["name"].Value;
        var @operator = match.Groups["operator"].Value;
        Expression expression = @operator switch
        {
            "+" => Expression.Add(leftExpression, rightExpression),
            "-" => Expression.Subtract(leftExpression, rightExpression),
            "*" => Expression.Multiply(leftExpression, rightExpression),
            "/" => Expression.Divide(leftExpression, rightExpression),
            _ => throw new Exception("Invalid operator")
        };

        expressions.Add(name, expression);
    }
}

var root = expressions["root"];
var lambda = Expression.Lambda<Func<long>>(root).Compile();
Console.WriteLine(lambda());