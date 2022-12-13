namespace day11_2;

public class Monkey
{
    public LinkedList<long> Items { get; init; }
    public Func<long, long> Operation { get; init; }
    public long Modulo { get; init; }
    public int TrueMonkey { get; init; }
    public int FalseMonkey { get; init; }
    public long Inspections { get; set; } = 0;

    public Monkey(LinkedList<long> items, Func<long, long> operation, long modulo, int trueMonkey, int falseMonkey)
    {
        Items = items;
        Operation = operation;
        Modulo = modulo;
        TrueMonkey = trueMonkey;
        FalseMonkey = falseMonkey;
    }
}