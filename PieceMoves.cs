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

        internal char[,] pieces;
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
            pieces = board.pieces;
            piece = PieceAt(s);

            GetPieceMoves();
        }

        private void GetPieceMoves()
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

            int[] coord = new int[2];
            switch (p.MoveType)
            {
                case EMOVEMENTTYPE.Combined:
                    moves.AddRange(GetStraightMoves());
                    moves.AddRange(GetDiagMoves());
                    break;
                case EMOVEMENTTYPE.Straight:
                    moves.AddRange(GetStraightMoves());
                    break;
                case EMOVEMENTTYPE.Diagonal:
                    moves.AddRange(GetDiagMoves());
                    break;
                case EMOVEMENTTYPE.LShape:
                    moves.AddRange(GetLShapeMoves());
                    break;
                case EMOVEMENTTYPE.Pawn:
                    moves.AddRange(GetPawnMoves());
                    break;
            }
        }

        private List<int[]> GetStraightMoves()
        {
            //for (int i = 1; i<p.MoveSize; i++)
            //{
            //    coord[0] = i;
            //    if (PieceAt())
            //}
            return new List<int[]>();
        }
        private List<int[]> GetDiagMoves()
        {
            //for (int i = 1; i<p.MoveSize; i++)
            //{
            //    coord[0] = i;
            //    if (PieceAt())
            //}
            return new List<int[]>();
        }
        private List<int[]> GetLShapeMoves()
        {
            //for (int i = 1; i<p.MoveSize; i++)
            //{
            //    coord[0] = i;
            //    if (PieceAt())
            //}
            return new List<int[]>();
        }
        private List<int[]> GetPawnMoves()
        {
            //for (int i = 1; i<p.MoveSize; i++)
            //{
            //    coord[0] = i;
            //    if (PieceAt())
            //}
            return new List<int[]>();
        }

        private bool IsEmpty(int[] s)
        {
            if (PieceAt(s) == '\0')
                return false;
            else
                return true;
        }

        private bool IsFriendly(int[] s)
        {
            if (IsUpper(PieceAt(s)) == IsUpper(piece))
                return true;
            else
                return false;
        }
        private bool IsEnemy(int[] s)
        {
            if (IsUpper(PieceAt(s)) != IsUpper(piece))
                return false;
            else
                return true;
        }
        private char PieceAt(int[] a)
        {
            return pieces[a[0], a[1]];
        }
    }
}
