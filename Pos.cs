using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal struct Pos
    {
        internal int Row;
        internal int Column;
        internal Pos(int row, int column)
        {
            Row = row;
            Column = column;
        }
        internal static Pos Null
        {
            get { return new Pos(-1, -1); }
        }
    }
}
