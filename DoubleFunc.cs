using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal static class DoubleFunc
    {
        internal static double GetGreater(double a, double b)
        {
            if (a > b)
                return a;
            else
                return b;
        }
        internal static double GetLower(double a, double b)
        {
            if (a < b)
                return a;
            else
                return b;
        }
    }
}
