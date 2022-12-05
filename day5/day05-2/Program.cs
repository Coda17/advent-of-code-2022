var lines = File.ReadAllLines(args[0]);
var emptyLineIndex = lines.Select((l, i) => (l, i)).Where(x => string.IsNullOrEmpty(x.l)).Select(x => x.i).First();

var stackCount = lines[emptyLineIndex - 1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
var stacks = Enumerable.Range(0, stackCount).Select(_ => new Stack<char>()).ToArray();

for (var i = emptyLineIndex - 2; i >= 0; --i)
{
    var chunks = lines[i].Chunk(4)
        .Select(x => new string(x).Trim())
        // Remove brackets
        .Select(x => x == string.Empty ? (char?)null : x[1])
        .ToArray();

    foreach (var (chunk, j) in chunks.Select((item, index) => (item, index)))
    {
        if (chunk is not null)
        {
            stacks[j].Push(chunk.Value);
        }
    }
}

var procedure = lines.Skip(emptyLineIndex + 1).ToArray();
foreach (var step in procedure)
{
    var split = step.Split(' ')
        .Where(x => !x.Any(char.IsLetter))
        .Select(int.Parse)
        .Select((x, i) => i == 0 ? x : x - 1)
        .ToArray();

    var tempStack = new Stack<char>();
    foreach (var _ in Enumerable.Range(0, split[0]))
    {
        tempStack.Push(stacks[split[1]].Pop());
    }

    foreach (var _ in Enumerable.Range(0, split[0]))
    {
        stacks[split[2]].Push(tempStack.Pop());
    }
}

foreach (var stack in stacks)
{
    Console.Write(stack.Pop());
}

Console.Write('\n');