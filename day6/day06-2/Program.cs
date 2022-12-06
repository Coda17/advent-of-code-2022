foreach (var line in File.ReadLines(args[0]))
{
    var marker = 0;
    while (line.Length - 4 - marker > 0)
    {
        var fourteen = line.Substring(marker, 14);
        ++marker;

        if (fourteen.Distinct().Count() == 14)
        {
            marker += 13;
            break;
        }
    }


    Console.WriteLine(marker);
}