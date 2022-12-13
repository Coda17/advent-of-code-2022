var cycle = 1;
var x = 1;
var signalStrength = 0;
foreach (var line in File.ReadLines(args[0]))
{
    var split = line.Split(' ');
    var op = split[0];

    if (op == "addx")
    {
        ++cycle;
        if ((cycle - 20) % 40 == 0)
        {
            signalStrength += cycle * x;
        }

        var v = int.Parse(split[1]);
        x += v;
    }

    ++cycle;
    if ((cycle - 20) % 40 == 0)
    {
        signalStrength += cycle * x;
    }
}

Console.WriteLine(signalStrength);