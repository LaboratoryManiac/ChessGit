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
        internal char[,] pieces = new char[8,8];
        internal ECOLOUR turn = ECOLOUR.Null;
        internal bool[] castle = new bool[4]; //0 = K, 1 = Q, 2 = k, 3 = q
        internal int[] passant = new int[2];
        internal int halfm = 0;
        internal int fullm = 0;

        internal Board()
        {

        }
        internal Board(string fen)
        {
            //int fenpos = 0;
            EFENPOS fenpos = EFENPOS.Pieces;
            int[] boardpos = new int[2]; //a8(0)->h8(7)->b7(8) etc.

            char[,] pieces = new char[8,8];
            ECOLOUR turn = ECOLOUR.Null;
            bool[] castle = new bool[4];
            int[] passant = new int[2];
            int halfm = 0;
            int fullm = 0;

            foreach (char c in fen)
            {
                if (c == ' ')
                {
                    fenpos++;
                }
                else
                {
                    switch (fenpos)
                    {
                        case EFENPOS.Pieces:
                            if (IsPiece(c)) //piece
                            {
                                pieces[boardpos[0], boardpos[1]] = c;
                                boardpos = Increment2D(boardpos, 7);
                            }
                            else if (IsNum(c)) //blank spaces
                            {
                                for (int i = 0; i < GetNum(c); i++)
                                {
                                    boardpos = Increment2D(boardpos, 7);
                                }
                            }
                            break;
                        case EFENPOS.Turn:
                            turn = c switch
                            {
                                'w' => ECOLOUR.White,
                                'b' => ECOLOUR.Black,
                                _ => throw new Exception("Invalid turn colour, must be 'w' or 'b'"),
                            };
                            if (c == 'w')
                                turn = ECOLOUR.White;
                            else if (c == 'b')
                                turn = ECOLOUR.Black;
                            break;
                        case EFENPOS.Castle:
                            switch (c)
                            {
                                case '-':
                                    castle[0] = false;
                                    castle[1] = false;
                                    castle[2] = false;
                                    castle[3] = false;
                                    break;
                                case 'K':
                                    castle[0] = true;
                                    break;
                                case 'Q':
                                    castle[1] = true;
                                    break;
                                case 'k':
                                    castle[2] = true;
                                    break;
                                case 'q':
                                    castle[3] = true;
                                    break;
                                default:
                                    throw new Exception("Invalid castling");
                            }
                            break;
                        case EFENPOS.Passant:
                            if (c == '-')
                            {
                                passant[0] = -1;
                                passant[1] = -1;
                            }
                            else if (IsBetween(c, 96, 104)) //is a-h
                            {
                                passant[0] = CharToNum(c);
                            }
                            else if (IsSingleDigit(c)) //is 1-9
                            {
                                passant[1] = GetNum(c);
                            }
                            else
                                throw new Exception("Invalid en passant");
                            break;
                        case EFENPOS.HalfM:
                            if (IsNum(c))
                            {
                                if (halfm == 0)
                                {
                                    halfm += GetNum(c);
                                }
                                else
                                {
                                    halfm *= 10;
                                    halfm += GetNum(c);
                                }
                            }
                            else
                                throw new Exception("Invalid half move count");
                            break;
                        case EFENPOS.FullM:
                            if (IsNum(c))
                            {
                                if (fullm == 0)
                                {
                                    fullm += GetNum(c);
                                }
                                else
                                {
                                    fullm *= 10;
                                    fullm += GetNum(c);
                                }
                            }
                            else
                                throw new Exception("Invalid full move count");
                            break;
                    }
                }
            }

            if (!IsMax2D(boardpos, 7))
                throw new Exception("Invalid board state, check FEN");
            else
            {
                this.pieces = pieces;
                this.turn = turn;
                this.castle = castle;
                this.passant = passant;
                this.halfm = halfm;
                this.fullm = fullm;
            }
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
                return c is 'b' or 'k' or 'n' or 'p' or 'q' or 'r';
            }
            else
                return false;
        }

        internal static int[] IntToCoord(int i) //7 = 0,7  42 = 5,2
        {
            int[] a = new int[2];
            a[0] = i / 8; //IntPos[0] = row
            a[1] = i % 8; //IntPos[1] = column
            return a;
        }
        internal static int CoordToInt(int[] a)
        {
            int i = 0;
            i += a[0] * 8;
            i += a[1];
            return i;
        }

        internal bool Move(int[] pos1, int[] pos2)
        {
            if (LegalMoves(pos1).moves.Any(p => p.SequenceEqual(pos2)) /*check if piece can move to destination*/)
            {
                char piece1 = pieces[pos1[0], pos1[1]];
                char piece2 = pieces[pos2[0], pos2[1]];

                pieces[pos2[0], pos2[1]] = piece1;
                pieces[pos1[0], pos1[1]] = '\0';
                turn = turn switch
                {
                    ECOLOUR.Black => ECOLOUR.White,
                    ECOLOUR.White => ECOLOUR.Black,
                };
                // if piece moved was pawn or square moved to had piece (piece was captured)
                if (ToUpper(piece1) == 'P' || ToUpper(piece2) != '\0')
                    halfm = 0;
                //else reset
                else
                    halfm += 1;
                fullm += 1;
                return true;
            }
            return false;
        }

        internal PieceMoves LegalMoves(int[] pos)
        {
            return new PieceMoves(pos, this);
        }

        internal List<PieceMoves> AllLegalMoves()
        {
            List<PieceMoves> moves = new();

            List<int[]> StList = GetPieceSquares();
            foreach (int[] s in StList)
            {
                moves.Add(new PieceMoves(s, this));
            }

            return moves;
        }

        internal List<int[]> GetPieceSquares()
        {
            List<int[]> PieceSquares = new List<int[]>();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int[] a = new int[2];
                    a[0] = i;
                    a[1] = j;

                    if (pieces[i, j] != '\0')
                    {
                        PieceSquares.Add(a);
                    }
                }
            }

            return PieceSquares;
        }
    }
}
