using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using static Godot.RenderingDevice;

namespace Chess
{
	enum Players
	{
		WHITE,
		BLACK
	}

	struct Position: IEquatable<Position>
	{
		public int x; 
		public int y;
		public bool isWithinBounds { get => x >= 0 && x <= 15 && y >= 0 && y <= 15;}
		public Position(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
        public bool Equals(Position other)
        {
			return x == other.x && y == other.y;
        }
        public override bool Equals(object other)
        {
            if (other is Position)
            {
                Position other2 = (Position)other;
                return Equals(other2);
            }

            return false;
        }
		public override int GetHashCode()
		{
			return (x, y).GetHashCode();
		}
		public static bool operator ==(Position lhs, Position rhs)
        {
			if (lhs.x == rhs.x && lhs.y == rhs.y) return true;
            return false;
        }
        public static bool operator !=(Position lhs, Position rhs)
        {
            return !(lhs == rhs);
        }
		public static Position operator +(Position lhs, Position rhs)
		{
			return new Position(lhs.x + rhs.x, lhs.y + rhs.y);
		}
		public static Position operator ~(Position rhs)
		{
			//gets the same time from the other side of the board
			return new Position(rhs.x, 15 - rhs.y);
		}
		public static Position operator ^(Position lhs, Directions dir)
		{
			//steps in the direction
			Position delta = Ops.directions[dir];
			return lhs + delta;
		}
        public override string ToString()
        {
            return $"({x}, {y})";
        }
        private void ToNotation()
        {
            throw new NotImplementedException();
        }
    }

    struct Tile
	{
		Piece? piece = null;
		bool isMarked = false;
		int countDown = -1;
		//bool isOccupied; //this is supposed to be a {get}
						 //method for time machine activation and countdown
						 //method to take in piece
		public Tile()
		{
		}
	}

	struct Turn
	{
		Players current;
		List<Players> extraTurns;
		bool checkAllowed;
		//addExtraTurn
		//are we in Extra turn
		//rollover
	}
	struct Check {
		bool isInCheck;
		Players who;
		// playerIsInCheck(Move) => void
	}

	enum Directions
	{
		DOWN_LEFT = 1,
		DOWN = 2,
		DOWN_RIGHT = 3,
		LEFT = 4,
		RIGHT = 6,
		UP_LEFT = 7,
		UP = 8,	
		UP_RIGHT = 9,
	}

	static class Ops 
	{
		public static Dictionary<Directions, Position> directions = new Dictionary<Directions, Position>()
		{
			[Directions.DOWN_LEFT] = new Position(-1, -1),
			[Directions.DOWN] = new Position(0, -1),
			[Directions.DOWN_RIGHT] = new Position(1, -1),
			[Directions.LEFT] = new Position(-1, 0),
			[Directions.RIGHT] = new Position(1, 0),
			[Directions.UP_LEFT] = new Position(-1, 1),
			[Directions.UP] = new Position(0, 1),
			[Directions.UP_RIGHT] = new Position(1, 1)
		};
		public static List<Position> getVertical (Position start, int limit = 15, int skip = 0) { 
			List<Position> vertical = new List<Position>();
			int upLimit = limit == 15 ? 15 - start.y : limit;
            int downLimit = limit == 15 ? 15 - upLimit : limit;
			vertical.AddRange(getDir(start, Directions.DOWN, upLimit, skip));
			vertical.AddRange(getDir(start, Directions.UP, downLimit, skip));
			return vertical;
        }
        public static List<Position> getHorizontal(Position start, int limit = 15, int skip = 0)
        {
            List<Position> horizontal = new List<Position>();
            int rightLimit = limit == 15 ? 15 - start.x : limit;
            int leftLimit = limit == 15 ? 15 - rightLimit : limit;
            horizontal.AddRange(getDir(start, Directions.RIGHT, rightLimit, skip));
            horizontal.AddRange(getDir(start, Directions.LEFT, leftLimit, skip));
            return horizontal;
        }
		public static List<Position> getRook(Position start, int limit = 15, int skip = 0)
		{
			List<Position> positions = new List<Position>();
			positions.AddRange(getHorizontal(start, limit, skip));
			positions.AddRange(getVertical(start, limit, skip));
			return positions;
		}
		public static List<Position> getBishop(Position start, int limit = 15, int skip = 0)
		{
            List<Position> positions = new List<Position>();
            positions.AddRange(getDir(start, Directions.DOWN_LEFT, limit,skip));
            positions.AddRange(getDir(start, Directions.DOWN_RIGHT, limit, skip));
            positions.AddRange(getDir(start, Directions.UP_LEFT, limit, skip));
            positions.AddRange(getDir(start, Directions.UP_RIGHT, limit, skip));
            return positions;
		}
		public static List<Position> getKing(Position start, int limit = 15, int skip = 0)
		{
            List<Position> positions = new List<Position>();
            foreach ((Directions dir, Position pos) in directions)
            {
				positions.Add(start ^ dir);
            }
			return positions;
        }

        public static List<Position> getDir(Position start, Directions dir, int limit = 15, int skip = 0) { 
			List<Position> positions = new List<Position>();
            for (int i = skip; i < limit; i++)
            {
				start = start ^ dir;
				if (start.isWithinBounds) positions.Add(start);
				else break;
            }
			return positions;
        }
    }
}
