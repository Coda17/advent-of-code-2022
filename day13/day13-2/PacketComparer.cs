namespace day13_2;

public class PacketComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        var (ordered, _, _) = IsOrdered(x!, y!);
        switch (ordered)
        {
            case Result.Ordered: return -1;
            case Result.NotOrdered: return 1;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    private static (Result, int, int) IsOrdered(string left, string right)
    {
        var leftIndex = 0;
        var rightIndex = 0;
        while (leftIndex < left.Length && rightIndex < right.Length)
        {
            var ordered = Result.Continue;
            var leftChars = 1;
            var rightChars = 1;

            if (char.IsDigit(left[leftIndex]) && char.IsDigit(right[leftIndex]))
            {
                (var leftInt, leftChars) = GetInteger(left, leftIndex);
                (var rightInt, rightChars) = GetInteger(right, rightIndex);

                if (leftInt < rightInt)
                {
                    ordered = Result.Ordered;
                }

                if (rightInt < leftInt)
                {
                    return (Result.NotOrdered, rightChars, leftChars);
                }
            }
            else if (left[leftIndex] == '[' && right[rightIndex] == '[')
            {
                var leftList = GetList(left, leftIndex);
                var rightList = GetList(right, rightIndex);

                (ordered, leftChars, rightChars) = IsOrdered(leftList, rightList);
                leftChars += 2;
                rightChars += 2;
            }
            else if (left[leftIndex] == '[' && char.IsDigit(right[rightIndex]))
            {
                var leftList = GetList(left, leftIndex);
                (var rightList, rightChars) = GetInteger(right, rightIndex);

                (ordered, leftChars, _) = IsOrdered(leftList, $"{rightList}");
            }
            else if (char.IsDigit(left[leftIndex]) && right[rightIndex] == '[')
            {
                (var leftList, leftChars) = GetInteger(left, leftIndex);
                var rightList = GetList(right, rightIndex);

                (ordered, _, rightChars) = IsOrdered($"{leftList}", rightList);
            }

            switch (ordered)
            {
                case Result.NotOrdered:
                    return (ordered, leftChars, rightChars);
                case Result.Ordered:
                    return (ordered, leftChars, rightChars);
                case Result.Continue:
                    // do nothing
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            leftIndex += leftChars;
            rightIndex += rightChars;
        }

        if (leftIndex == left.Length && rightIndex != right.Length)
        {
            return (Result.Ordered, left.Length, right.Length);
        }

        if (rightIndex == right.Length && leftIndex != left.Length)
        {
            return (Result.NotOrdered, left.Length, right.Length);
        }

        return (Result.Continue, left.Length, right.Length);
    }

    private static (int, int) GetInteger(string str, int index)
    {
        var last = index;
        while (last + 1 < str.Length && char.IsDigit(str[last + 1]))
        {
            ++last;
        }

        return (int.Parse(str.Substring(index, last - index + 1)), last - index + 1);
    }

    private static string GetList(string str, int index)
    {
        var count = 1;
        var last = index + 1;
        for (; index < str.Length && count > 0; ++last)
        {
            if (str[last] == '[')
            {
                ++count;
            }
            else if (str[last] == ']')
            {
                --count;
            }
        }

        return str.Substring(index + 1, last - index - 2);
    }
}