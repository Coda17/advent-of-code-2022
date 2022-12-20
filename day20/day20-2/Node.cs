namespace day20_2;

public class Node
{
    public Node? Previous { get; set; }
    public Node? Next { get; set; }
    public long Value { get; set; }
    public override string ToString() => Value.ToString();
}