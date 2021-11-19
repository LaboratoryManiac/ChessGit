using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal static class CharFunc
    {
        internal static int ToNum(char c) //for letters, a = 0, b = 1...
        {
            c = ToUpper(c);
            return c - 65;
        }
        internal static int GetNum(char c) //should use with IsNum
        {
            return c - 48;
        }
        internal static char ToUpper(char c) //should use with IsAlpha
        {
            if (c > 90)
                c = (char) (c - 32);
            return c;
        }
        internal static char ToLower(char c) //should use with IsAlpha
        {
            if (c < 90)
                c = (char)(c + 32);
            return c;
        }
        internal static bool IsUpper(char c)
        {
            if (c > 64 && c < 91)
                return true;
            else
                return false;
        }
        internal static bool IsAlpha(char c)
        {
            if ((c > 64 && c < 91) || (c > 96 && c < 123))
                return true;
            else
                return false;
        }

        internal static bool IsNum(char c)
        {
            if (c > 47 && c < 58)
                return true;
            else
                return false;
        }
        internal static bool IsEqual(char c, char d)
        {
            if (c == d)
                return true;
            else
                return false;
        }
    }
}
