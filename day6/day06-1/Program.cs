foreach (var line in File.ReadLines(args[0]))
{
    var marker = 0;
    while (line.Length - 4 - marker > 0)
    {
        var four = line.Substring(marker, 4);
        ++marker;

        if (four.Distinct().Count() == 4)
        {
            marker += 3;
            break;
        }
    }


    Console.WriteLine(marker);
}