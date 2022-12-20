using day20_2;

const long decryptionKey = 811589153L;
var lines = File.ReadAllLines(args[0])
    .Select((x, i) => (Value: long.Parse(x) * decryptionKey, Index: i))
    .ToArray();

var nodes = new List<Node>(lines.Length);
Node? previous = null;
foreach (var (value, index) in lines)
{
    var node = new Node
    {
        Value = value,
        Previous = previous
    };

    if (index > 0)
    {
        previous!.Next = node;
    }

    nodes.Add(node);

    previous = node;
}

nodes[lines.Length - 1].Next = nodes[0];
nodes[0].Previous = nodes[lines.Length - 1];

//Print(nodes[0], nodes.Count);
for (var i = 0; i < 10; ++i)
{
    foreach (var node in nodes)
    {
        var value = Math.Abs(node.Value) % (nodes.Count - 1);
        for (var j = 0; j < value; ++j)
        {
            MoveNode(node.Value > 0 ? node : node.Previous!);
        }
    }

    //Print(nodes[0], nodes.Count);
}

var groveCoordinates = GetGroveCoordinates(nodes);
Console.WriteLine(
    $"{groveCoordinates.x} + {groveCoordinates.y} + {groveCoordinates.z} = {groveCoordinates.x + groveCoordinates.y + groveCoordinates.z}");

static void MoveNode(Node node)
{
    var previous = node.Previous!;
    var next = node.Next!;
    var after = next.Next!;

    previous.Next = next;
    after.Previous = node;

    node.Previous = next;
    node.Next = after;

    next.Previous = previous;
    next.Next = node;
}

static (long x, long y, long z) GetGroveCoordinates(IEnumerable<Node> nodes)
{
    var node = nodes.First(x => x.Value == 0);
    long x = 0, y = 0, z = 0;
    for (var i = 0; i < 30000; ++i)
    {
        if (i == 1000)
        {
            x = node!.Value;
        }

        if (i == 2000)
        {
            y = node!.Value;
        }

        if (i == 3000)
        {
            z = node!.Value;
        }

        node = node!.Next;
    }

    return (x, y, z);
}

static void Print(Node node, int count)
{
    for (var i = 0; i < count; ++i)
    {
        if (i != 0)
        {
            Console.Write(", ");
        }

        Console.Write(node);
        node = node.Next!;
    }

    Console.Write($"{Environment.NewLine}{Environment.NewLine}");
}