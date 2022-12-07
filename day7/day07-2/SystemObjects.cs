namespace day07_2;

public interface ISystemObject
{
    string Name { get; }
    public Directory? Parent { get; }

    int GetSize();
}

public class File : ISystemObject
{
    public string Name { get; }
    public int Size { get; }
    public Directory? Parent { get; }

    public File(string name, int size, Directory? parent)
    {
        Name = name;
        Size = size;
        Parent = parent;
    }

    public int GetSize() => Size;
    public override string ToString() => $"{Name} (file, size={Size})";
}

public class Directory : ISystemObject
{
    public string Name { get; }
    public Directory? Parent { get; }

    public HashSet<ISystemObject> SystemObjects { get; } = new();

    public Directory(string name, Directory? parent)
    {
        Name = name;
        Parent = parent;
    }

    private int? _size;
    public int GetSize()
    {
        // Only calculate once because a directory is never updated after the size is queried.
        _size ??= SystemObjects.Aggregate(0, (i, o) => i + o.GetSize());
        return _size.Value;
    }

    public override string ToString() => $"{Name} (dir)";
}