using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal static class IntFunc
    {
        internal static bool IsEven(int i)
        {
            if ((i % 2) == 0)
                return true;
            else
                return false;
        }
        internal static bool IsMult(int i, int mult)
        {
            if (i == 0)
                return false;
            int j = i % mult;
            if (j == 0)
                return true;
            else
                return false;
        }
        internal static int NextMult(int i, int mult)
        {
            int j = i % mult;
            j = mult - j;
            i += j;

            return i;
        }
        internal static int Positive(int i)
        {
            if (i < 0)
                return i * -1;
            else
                return i;
            //i *= i;
            //i = GetIntRoot(i);
            //return i;
        }
        internal static int GetIntRoot(int i) //might take a while :D
        {
            for (int j = 0; j < i; j++)
            {
                if (j * j == i)
                    return j;
            }
            return i;
        }

        internal static int GreaterOf(int i, int j)
        {
            if (i > j)
                return i;
            else
                return j;
        }

        internal static Pos Increment2D(Pos pos, int cap)
        {
            if (pos.Column == cap)
            {
                pos.Column = 0;
                pos.Row += 1;
            }
            else
            {
                pos.Column += 1;
            }
            return pos;
        }

        internal static bool IsMax2D(Pos a, int cap)
        {
            if (a.Row > cap)
                return true;
            else
                return false;
        }
    }
}
