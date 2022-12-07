using Directory = day07_1.Directory;
using File = day07_1.File;

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

Console.WriteLine(DeleteCount(system));

static int DeleteCount(Directory directory)
{
    var deleteSize = directory.SystemObjects.OfType<Directory>().Sum(DeleteCount);

    var size = directory.GetSize();
    if (size < 100000)
    {
        deleteSize += size;
    }

    return deleteSize;
}

static bool IsCd(IReadOnlyList<string> split) => split.Count == 3 && split[0] == "$" && split[1] == "cd";
static bool IsLs(IReadOnlyList<string> split) => split.Count == 2 && split[0] == "$" && split[1] == "ls";
static bool IsDir(IReadOnlyList<string> split) => split.Count == 2 && split[0] == "dir";