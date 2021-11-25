using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Chess.CharFunc;

namespace Chess
{
    internal enum ECOLOUR { Null, Black, White }

    internal enum EPIECE { Null, King, Queen, Rook, Bishop, Knight, Pawn }
    internal class Piece
    {
        internal ECOLOUR Colour = ECOLOUR.Null;
        internal EPIECE Type = EPIECE.Null;

        internal Piece(char c)
        {
            if (c == '\0')
                Colour = ECOLOUR.Null;
            else if (IsUpper(c))
                Colour = ECOLOUR.White;
            else if (IsLower(c))
                Colour = ECOLOUR.Black;
            else
                throw new Exception("Invalid piece colour, must be standard alphabet");
            Type = ToUpper(c) switch
            {
                'K' => EPIECE.King,
                'Q' => EPIECE.Queen,
                'R' => EPIECE.Rook,
                'B' => EPIECE.Bishop,
                'N' => EPIECE.Knight,
                'P' => EPIECE.Pawn,
                '\0' => EPIECE.Null,
                _ => throw new Exception("Invalid piece type, must be king/queen/rook/bishop/knight/pawn"),
            };
        }

        internal int Value
        {
            get
            {
                return Type switch
                {
                    EPIECE.Queen => 9,
                    EPIECE.Rook => 5,
                    EPIECE.Bishop => 3,
                    EPIECE.Knight => 3,
                    EPIECE.Pawn => 1,
                    _ => -100,//error if used
                };
            }
        }

        internal static Piece Null
        {
            get
            {
                return new Piece('\0');
            }
        }
    }
}
