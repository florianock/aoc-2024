namespace AdventOfCode.Utils;

public class Grid
{
    internal enum Direction
    {
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    }
    
    
    public static List<(int, int)> GetNeighbors(int r, int c, bool includeDiagonals = false)
    {
        if (includeDiagonals)
            return
            [
                (r - 1, c), (r - 1, c + 1), (r, c + 1), (r + 1, c + 1),
                (r + 1, c), (r + 1, c - 1), (r, c - 1), (r - 1, c - 1)
            ];
        return [(r - 1, c), (r, c + 1), (r + 1, c), (r, c - 1)];
    }
}


