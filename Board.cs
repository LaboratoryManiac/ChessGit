using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Chess.IntFunc;
using static Chess.CharFunc;

namespace Chess
{
    enum EFENPOS { Pieces, Turn, Castle, Passant, HalfM, FullM }
    enum ECOLOUR { Null, Black, White }
    enum EPIECE { Null, King, Queen, Rook, Bishop, Knight, Pawn }

    internal class Board
    {
        internal char[] pos = new char[64];
        internal ECOLOUR turn = ECOLOUR.Null;
        internal char[] castle = new char[4];
        internal int passant = 0;
        internal int halfm = 0;
        internal int fullm = 0;
        internal Board(string fen)
        {
            //int fenpos = 0;
            EFENPOS fenpos = EFENPOS.Pieces;
            int boardpos = 0; //a8(0)->h8(7)->b7(8) etc.

            char[] pos = new char[64];
            ECOLOUR turn = ECOLOUR.Null;
            char[] castle = new char[4];
            int passant = 0;
            int halfm = 0;
            int fullm = 0;

            fen = fen.Replace(" ", "");
            foreach (char c in fen)
            {
                switch (fenpos)
                {
                    case EFENPOS.Pieces:
                        if (IsPiece(c)) //piece
                        {
                            pos[boardpos] = c;
                            boardpos++;
                        }
                        else if (IsNum(c)) //blank spaces
                        {
                            boardpos += GetNum(c);
                        }
                        if (boardpos == 64)
                            fenpos++;
                        break;
                    case EFENPOS.Turn:
                        if (c == 'w')
                            turn = ECOLOUR.White;
                        else if (c == 'b')
                            turn = ECOLOUR.Black;
                        fenpos++;
                        break;
                    case EFENPOS.Castle:
                        if (c == '-') //no castling
                        {
                            castle[0] = '-';
                            castle[1] = '-';
                            castle[2] = '-';
                            castle[3] = '-';
                        }
                        else
                        {
                            passant++;
                            if (passant == 4)
                            {
                                passant = 0;
                                fenpos++;
                            }
                        }
                        break;
                    case EFENPOS.Passant:
                        if (IsEqual(c, '-'))
                        {
                            passant = 0;
                            fenpos++;
                        }
                        else if (passant == 0) //2 step, first passant = 
                        {
                            passant = ToNum(c) + 1;
                        }
                        else
                        {
                            passant = GetPosFromCoord(passant, GetNum(c));
                        }
                        break;
                    case EFENPOS.HalfM:
                        fenpos++;
                        break;
                    case EFENPOS.FullM:
                        fenpos++;
                        break;
                }
            }
            if (boardpos != 64)
                throw new Exception("Invalid board state, check FEN");
            else if (turn == ECOLOUR.Null)
                throw new Exception("Invalid turn colour, must be 'w' or 'b'");
            this.pos = pos;
            this.turn = turn;
            this.castle = castle;
            this.passant = passant;
            this.halfm = halfm;
            this.fullm = fullm;
        }

        static int GetPosFromCoord(int a, int b) //must be "e5" etc.
        {
            int i = Positive(b - 8);
            i *= 8;
            i += a - 1; //takes a = 1, b = 2 so must subtract 1
            return i;
        }

        static bool IsPiece(char c) //b k n p q r
        {
            if (IsAlpha(c))
            {
                c = ToLower(c);
                if (c == 'b' || c == 'k' || c == 'n' || c == 'p' || c == 'q' || c == 'r' )
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        internal static int[] IntPos(int i) //7 = 0,7  42 = 5,2
        {
            int[] a = new int[2];
            a[0] = i / 8; //IntPos[0] = row
            a[1] = i % 8; //IntPos[1] = column
            return a;
        }
    }
}
