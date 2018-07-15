using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiplayerGames.Shared
{
    public class IntVector2
    {
        public static IntVector2 operator +(IntVector2 a, IntVector2 b)
        {
            return new IntVector2 (a.X + b.X, a.Y + b.Y);
        }
        public int X { get; set; }
        public int Y { get; set; }
        public IntVector2 (int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public IntVector2 Clone()
        {
            return new IntVector2(this.X, this.Y);
        }
        public override bool Equals(object obj)
        {
            IntVector2 vectorObj = obj as IntVector2;
            if (vectorObj == null)
                return false;
            else
                return this.X == vectorObj.X && this.Y == vectorObj.Y;
        }
    }
}
