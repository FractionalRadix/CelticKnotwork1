namespace CelticKnotwork1
{
    enum Direction { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest };

    class DirectionUtils
    {
        public static bool SameAxis(Direction dir1, Direction dir2)
        {
            if (dir1.Equals(dir2))
            {
                return true;
            }

            switch (dir1)
            {
                case Direction.North:
                    return dir2.Equals(Direction.South);
                case Direction.NorthEast:
                    return dir2.Equals(Direction.SouthWest);
                case Direction.East:
                    return dir2.Equals(Direction.West);
                case Direction.SouthEast:
                    return dir2.Equals(Direction.NorthWest);
                case Direction.South:
                    return dir2.Equals(Direction.North);
                case Direction.SouthWest:
                    return dir2.Equals(Direction.NorthEast);
                case Direction.West:
                    return dir2.Equals(Direction.East);
                case Direction.NorthWest:
                    return dir2.Equals(Direction.SouthEast);
            }

            // Should never reach here...
            //TODO?+ Issue an error
            return false;
        }
    }
}