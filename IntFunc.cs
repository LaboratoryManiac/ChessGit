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

        internal static int[] Increment2D(int[] a, int cap) //only for int[2]
        {
            if (a[1] == cap)
            {
                a[1] = 0;
                a[0] += 1;
            }
            else
            {
                a[1] += 1;
            }
            return a;
        }

        internal static bool IsMax2D(int[] a, int cap)
        {
            if (a[0] == cap + 1)
                return true;
            else
                return false;
        }
    }
}
