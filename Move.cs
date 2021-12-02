using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal class Move
    {
        internal readonly int[] start = new int[2];
        internal readonly int[] end = new int[2];

        internal Move(int[] start, int[] end)
        {
            this.start = start;
            this.end = end;
        }
    }
}
