var x = 1;
// Leave extra room because I'm lazy.
var crt = Enumerable.Range(0, 7).Select(_ => string.Empty).ToArray();
var crtRowIndex = 0;

foreach (var line in File.ReadLines(args[0]))
{
    var split = line.Split(' ');
    var op = split[0];

    var v = 0;
    if (op == "addx")
    {
        v = int.Parse(split[1]);
    }

    if (Math.Abs(crt[crtRowIndex].Length - x) <= 1)
    {
        crt[crtRowIndex] += '#';
    }
    else
    {
        crt[crtRowIndex] += '.';
    }

    Console.WriteLine($"Start cycle   {crt[crtRowIndex].Length + crtRowIndex * 40}: begin executing {line}");
    Console.WriteLine($"During cycle  {crt[crtRowIndex].Length + crtRowIndex * 40}: CRT draws pixel in position {crt[crtRowIndex].Length - 1}");
    Console.WriteLine($"Current CRT row: {crt[crtRowIndex]}");

    if (crt[crtRowIndex].Length == 40)
    {
        ++crtRowIndex;
    }

    if (op == "noop")
    {
        Console.WriteLine($"End of cycle  {crt[crtRowIndex].Length + crtRowIndex * 40}: finish executing {line}\n");
        continue;
    }

    if (Math.Abs(crt[crtRowIndex].Length - x) <= 1)
    {
        crt[crtRowIndex] += '#';
    }
    else
    {
        crt[crtRowIndex] += '.';
    }

    Console.WriteLine($"\nDuring cycle  {crt[crtRowIndex].Length + crtRowIndex * 40}: CRT draws pixel in position {crt[crtRowIndex].Length - 1}");
    Console.WriteLine($"Current CRT row: {crt[crtRowIndex]}");

    x += v;

    Console.WriteLine($"End of cycle  {crt[crtRowIndex].Length + crtRowIndex * 40}: finish executing {line} (Register X is now {x})\n");

    if (crt[crtRowIndex].Length == 40)
    {
        ++crtRowIndex;
    }
}

foreach (var row in crt.Take(6))
{
    Console.WriteLine(row);
}