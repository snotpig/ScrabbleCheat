using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScrabbleCheat
{
    // structure for square on board
    public struct _square
    {
        public int row;
        public int col;

        public _square(int col, int row)
        {
            this.col = col;
            this.row = row;
        }

        public static bool operator ==(_square a, _square b)
        {
            return ((a.row == b.row) && (a.col == b.col));
        }

        public static bool operator !=(_square a, _square b)
        {
            return ((a.row != b.row) || (a.col != b.col));
        }

        public override bool Equals(object ob)
        {
            if (!(ob is _square)) return false;
            _square sq = (_square)ob;
            return ((row == sq.row) && (col == sq.col));
        }

        public override int GetHashCode()
        {
            return row * col;
        }
    };
}
