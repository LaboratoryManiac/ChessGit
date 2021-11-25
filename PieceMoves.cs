using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Chess.CharFunc;

namespace Chess
{
    internal class PieceMoves
    {
        internal int[] start;
        internal List<int[]> moves;

        internal Board board;
        internal char piece;

        internal static PieceType King = new PieceType(EMOVEMENTTYPE.Combined, 1);
        internal static PieceType Queen = new PieceType(EMOVEMENTTYPE.Combined, 7);
        internal static PieceType Rook = new PieceType(EMOVEMENTTYPE.Straight, 7);
        internal static PieceType Bishop = new PieceType(EMOVEMENTTYPE.Diagonal, 7);
        internal static PieceType Knight = new PieceType(EMOVEMENTTYPE.LShape, 3);
        internal static PieceType Pawn = new PieceType(EMOVEMENTTYPE.Pawn, 1);

        internal PieceMoves(int[] s, Board board)
        {
            start = s;
            this.board = board;
            piece = PieceAt(s);

            moves = GetPieceMoves();
        }

        private List<int[]> GetPieceMoves()
        {
            List<int[]> l = new();

            if ((board.turn == ECOLOUR.White && IsUpper(piece)) || (board.turn == ECOLOUR.Black && !IsUpper(piece))) //only generate moves if it's pieces' turn
            {
                PieceType p = ToUpper(piece) switch
                {
                    'K' => King,
                    'Q' => Queen,
                    'R' => Rook,
                    'B' => Bishop,
                    'N' => Knight,
                    'P' => Pawn,
                    _ => throw new Exception("Invalid piece"),
                };

                switch (p.MoveType)
                {
                    case EMOVEMENTTYPE.Combined:
                        l.AddRange(GetStraightMoves(p.MoveSize));
                        l.AddRange(GetDiagMoves(p.MoveSize));
                        break;
                    case EMOVEMENTTYPE.Straight:
                        l.AddRange(GetStraightMoves(p.MoveSize));
                        break;
                    case EMOVEMENTTYPE.Diagonal:
                        l.AddRange(GetDiagMoves(p.MoveSize));
                        break;
                    case EMOVEMENTTYPE.LShape:
                        l.AddRange(GetLShapeMoves(p.MoveSize));
                        break;
                    case EMOVEMENTTYPE.Pawn:
                        l.AddRange(GetPawnMoves(p.MoveSize));
                        break;
                }
            }
            return l;
        }

        private List<int[]> GetStraightMoves(int mS)
        {
            List<int[]> l = new();

            int[] coord = new int[2];
            coord[1] = start[1];
            //forward
            for (int i = start[0] - 1; i >= 0; i--)
            {
                coord[0] = i;
                char c = PieceAt(coord);

                if (!IsFriendly(c)) //can be rearranged to if(IsEmpty) add elseif(IsEnemy) add+break else break
                    l.Add((int[])coord.Clone());
                if (!IsEmpty(c))
                    break;
            }
            //back
            for (int i = start[0] + 1; i <= mS; i++)
            {
                coord[0] = i;
                char c = PieceAt(coord);

                if (!IsFriendly(c)) //can be rearranged to if(IsEmpty) add elseif(IsEnemy) add+break else break
                    l.Add((int[])coord.Clone());
                if (!IsEmpty(c))
                    break;
            }
            coord[0] = start[0];
            //left
            for (int i = start[1] - 1; i >= 0; i--)
            {
                coord[1] = i;
                char c = PieceAt(coord);

                if (!IsFriendly(c)) //can be rearranged to if(IsEmpty) add elseif(IsEnemy) add+break else break
                    l.Add((int[])coord.Clone());
                if (!IsEmpty(c))
                    break;
            }
            //right
            for (int i = start[1] + 1; i <= mS; i++)
            {
                coord[1] = i;
                char c = PieceAt(coord);

                if (!IsFriendly(c)) //can be rearranged to if(IsEmpty) add elseif(IsEnemy) add+break else break
                    l.Add((int[])coord.Clone());
                if (!IsEmpty(c))
                    break;
            }
            return l;
        }
        private List<int[]> GetDiagMoves(int mS)
        {
            //for (int i = 1; i<p.MoveSize; i++)
            //{
            //    coord[0] = i;
            //    if (PieceAt())
            //}
            return new List<int[]>();
        }
        private List<int[]> GetLShapeMoves(int mS)
        {
            //for (int i = 1; i<p.MoveSize; i++)
            //{
            //    coord[0] = i;
            //    if (PieceAt())
            //}
            return new List<int[]>();
        }
        private List<int[]> GetPawnMoves(int mS)
        {
            //for (int i = 1; i<p.MoveSize; i++)
            //{
            //    coord[0] = i;
            //    if (PieceAt())
            //}
            return new List<int[]>();
        }

        private bool IsEmpty(char c)
        {
            if (c == '\0')
                return true;
            else
                return false;
        }

        private bool IsFriendly(char c)
        {
            if (IsUpper(c) == IsUpper(piece)) //black squares and empty squares regarded as friendly - not as intended
                return true;
            else
                return false;
        }
        private bool IsEnemy(char c)
        {
            if (IsUpper(c) != IsUpper(piece))
                return false;
            else
                return true;
        }
        private char PieceAt(int[] a)
        {
            return board.pieces[a[0], a[1]];
        }
    }
}
