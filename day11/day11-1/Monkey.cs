namespace day11_1;

public class Monkey
{
    public LinkedList<long> Items { get; init; }
    public Func<long, long> Operation { get; init; }
    public Func<long, int> Test { get; init; }
    public long Inspections { get; set; } = 0;

    public Monkey(LinkedList<long> items, Func<long, long> operation, Func<long, int> test)
    {
        Items = items;
        Operation = operation;
        Test = test;
    }
}