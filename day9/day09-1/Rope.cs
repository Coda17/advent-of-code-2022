namespace day09_1;

public class Rope
{
    public int HeadX { get; private set; }
    public int HeadY { get; private set; }

    public int TailX { get; private set; }
    public int TailY { get; private set; }

    public void MoveHead(char direction)
    {
        switch (direction)
        {
            case 'L':
                HeadX--;
                if (TailX - HeadX > 1)
                {
                    TailX--;
                    TailY = HeadY != TailY ? HeadY : TailY;
                }
                break;
            case 'R':
                HeadX++;
                if (HeadX - TailX > 1)
                {
                    TailX++;
                    TailY = HeadY != TailY ? HeadY : TailY;
                }
                break;
            case 'U':
                HeadY++;
                if (HeadY - TailY > 1)
                {
                    TailY++;
                    TailX = HeadX != TailX ? HeadX : TailX;
                }
                break;
            case 'D':
                HeadY--;
                if (TailY - HeadY > 1)
                {
                    TailY--;
                    TailX = HeadX != TailX ? HeadX : TailX;
                }
                break;
            default:
                throw new ApplicationException("Shouldn't happen");
        }
    }
}