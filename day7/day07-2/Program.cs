using Directory = day07_2.Directory;
using File = day07_2.File;

var system = new Directory("/", null);
var cd = system;

foreach (var line in System.IO.File.ReadLines(args[0]))
{
    var split = line.Split(' ');
    if (IsCd(split))
    {
        switch (split[2])
        {
            case "/":
                cd = system;
                continue;
            case "..":
                cd = cd!.Parent;
                continue;
            default:
                cd = cd!.SystemObjects.First(x => x.Name == split[2]) as Directory;
                break;
        }
    }
    else if (IsLs(split))
    {
        // Ignore
    }
    else if (IsDir(split))
    {
        cd!.SystemObjects.Add(new Directory(split[1], cd));
    }
    else
    {
        cd!.SystemObjects.Add(new File(split[1], int.Parse(split[0]), cd));
    }
}

var totalSize = system.GetSize();
var requiredSpace = 30000000 - (70000000 - totalSize);
var toDelete = DirectoryToDelete(system, system);
Console.WriteLine($"{toDelete.Name}: {toDelete.GetSize()}");

Directory DirectoryToDelete(Directory best, Directory directory)
{
    if (directory.GetSize() < requiredSpace)
    {
        return best;
    }

    var currentSize = directory.GetSize();
    if (currentSize >= requiredSpace && currentSize < best.GetSize())
    {
        best = directory;
    }

    return directory.SystemObjects.OfType<Directory>()
        .Aggregate(best, DirectoryToDelete);
}

static bool IsCd(IReadOnlyList<string> split) => split.Count == 3 && split[0] == "$" && split[1] == "cd";
static bool IsLs(IReadOnlyList<string> split) => split.Count == 2 && split[0] == "$" && split[1] == "ls";
static bool IsDir(IReadOnlyList<string> split) => split.Count == 2 && split[0] == "dir";