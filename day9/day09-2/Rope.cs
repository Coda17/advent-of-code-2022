namespace day09_2;

public class Rope
{
    public IList<(int, int)> Knots { get; set; } = Enumerable.Range(0, 10).Select(_ => (0, 0)).ToArray();

    public void MoveHead(char direction)
    {
        var coordinates = Knots[0];
        switch (direction)
        {
            case 'L':
                Knots[0] = (coordinates.Item1 - 1, coordinates.Item2);
                break;
            case 'R':
                Knots[0] = (coordinates.Item1 + 1, coordinates.Item2);
                break;
            case 'U':
                Knots[0] = (coordinates.Item1, coordinates.Item2 + 1);
                break;
            case 'D':
                Knots[0] = (coordinates.Item1, coordinates.Item2 - 1);
                break;
            default:
                throw new ApplicationException("Shouldn't happen");
        }

        MoveKnot(1);
    }

    private void MoveKnot(int index)
    {
        var coordinates = Knots[index];
        var previous = Knots[index - 1];
        var xDif = previous.Item1 - coordinates.Item1;
        var absXDif = Math.Abs(xDif);
        var yDif = previous.Item2 - coordinates.Item2;
        var absYDif = Math.Abs(yDif);

        if (absXDif <= 1 && absYDif <= 1)
        {
            // Do nothing.
        }
        else if (absXDif == 2 && absYDif == 0)
        {
            var x = xDif > 0 ? coordinates.Item1 + 1 : coordinates.Item1 - 1;
            Knots[index] = (x, coordinates.Item2);
        }
        else if (absXDif == 0 && absYDif == 2)
        {
            var y = yDif > 0 ? coordinates.Item2 + 1 : coordinates.Item2 - 1;
            Knots[index] = (coordinates.Item1, y);
        }
        else
        {
            var x = xDif > 0 ? coordinates.Item1 + 1 : coordinates.Item1 - 1;
            var y = yDif > 0 ? coordinates.Item2 + 1 : coordinates.Item2 - 1;
            Knots[index] = (x, y);
        }

        if (index != 9)
        {
            MoveKnot(index + 1);
        }
    }
}