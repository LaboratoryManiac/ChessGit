using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    internal static class CharFunc
    {
        internal static int CharToNum(char c) //for letters, e.g. char a -> int 0, char b -> int 1...
        {
            c = ToUpper(c);
            return c - 65;
        }
        internal static int GetNum(char c) //for numbers, e.g. char 49[1] -> int 1
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
            if (IsBetween(c, 64, 91))
                return true;
            else
                return false;
        }
        internal static bool IsLower(char c)
        {
            if (IsBetween(c, 96, 123))
                return true;
            else
                return false;
        }
        internal static bool IsAlpha(char c)
        {
            if (IsBetween(c,64,91) || IsBetween(c, 96, 123))
                return true;
            else
                return false;
        }

        internal static bool IsNum(char c)
        {
            if (IsBetween(c, 47, 58))
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

        internal static bool IsBetween(char c, int i, int j)
        {
            if (c > i && c < j)
                return true;
            else
                return false;
        }

        internal static bool IsSingleDigit(char c) //single digit natural number (1-9)
        {
            if (IsBetween(c, 48, 58))
                return true;
            else
                return false;
        }

        internal static ECOLOUR ColourFromChar(char c)
        {
            if (IsUpper(c))
                return ECOLOUR.White;
            else
                return ECOLOUR.Black;
        }
    }
}
