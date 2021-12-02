using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Chess.IntFunc;
using static Chess.CharFunc;

namespace Chess
{
    internal enum EFENPOS { Pieces, Turn, Castle, Passant, HalfM, FullM }
    internal enum EWINNER { Null, Black, White, Draw }

    internal class Board
    {
        internal Piece[,] pieces = new Piece[8,8];
        internal ECOLOUR turn = ECOLOUR.Null;
        internal char[] castle = new char[4]; //0 = K, 1 = Q, 2 = k, 3 = q
        internal Pos passant = new();
        internal int halfm = 0;
        internal int fullm = 0;
        internal EWINNER winner = EWINNER.Null;

        internal Board()
        {

        }
        internal Board(string fen)
        {
            //int fenpos = 0;
            EFENPOS fenpos = EFENPOS.Pieces;
            Pos boardpos = new(); //a8(0)->h8(7)->b7(8) etc.

            Piece[,] pieces = new Piece[8,8];
            ECOLOUR turn = ECOLOUR.Null;
            char[] castle = new char[4]; //KQkq if all castling available
            Pos passant = new();
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
                                Piece p = new(c);
                                pieces[boardpos.Row, boardpos.Column] = p;
                                boardpos = Increment2D(boardpos, 7);
                            }
                            else if (IsNum(c)) //blank spaces
                            {
                                for (int i = 0; i < GetNum(c); i++)
                                {
                                    pieces[boardpos.Row, boardpos.Column] = Piece.Null;
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
                                    break;
                                case 'K':
                                    castle[0] = 'K';
                                    break;
                                case 'Q':
                                    castle[1] = 'Q';
                                    break;
                                case 'k':
                                    castle[2] = 'k';
                                    break;
                                case 'q':
                                    castle[3] = 'q';
                                    break;
                                default:
                                    throw new Exception("Invalid castling");
                            }
                            break;
                        case EFENPOS.Passant:
                            if (c == '-')
                            {
                                passant.Row = -1;
                                passant.Column = -1;
                            }
                            else if (IsBetween(c, 96, 104)) //is a-h
                            {
                                passant.Row = CharToNum(c);
                            }
                            else if (IsSingleDigit(c)) //is 1-9
                            {
                                passant.Column = GetNum(c);
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

        private static bool IsPiece(char c) //b k n p q r
        {
            if (IsAlpha(c))
            {
                c = ToLower(c);
                return c is 'b' or 'k' or 'n' or 'p' or 'q' or 'r';
            }
            else
                return false;
        }

        internal void MovePiece(Pos start, Pos end)
        {
            Piece piece1 = pieces[start.Row, start.Column];
            Piece piece2 = pieces[end.Row, end.Column];

            pieces[end.Row, end.Column] = piece1;
            pieces[start.Row, start.Column] = Piece.Null;
            turn = turn switch
            {
                ECOLOUR.Black => ECOLOUR.White,
                ECOLOUR.White => ECOLOUR.Black,
            };
            //if piece moved was pawn
            if (piece1.Type == EPIECE.Pawn)
            {//check if moved to end of board
                if ((start.Row == 0 && piece1.Colour == ECOLOUR.White) || (start.Row == 7 && piece1.Colour == ECOLOUR.Black))
                {//promote
                    piece1.Type = EPIECE.Queen; //TODO give option of other pieces - here or in move list? AI consideration
                    pieces[end.Row, end.Column] = piece1;
                }
                else if (Math.Abs(end.Row - start.Row) > 1)//check if pawn moved double
                {
                    Pos p = new(start.Row + PawnDirectionMult(piece1.Colour), start.Column);//square 1 up from start (1 down from end)

                    passant = p;//record as en passant capture option
                }
                else if (end.Equals(passant))//if taken en passant
                {
                    Pos p = new(end.Row - PawnDirectionMult(piece1.Colour), end.Column);//square en passanted piece is on

                    pieces[p.Row, p.Column] = Piece.Null;
                }
                else
                {
                    passant = Pos.Null;//remove current en passant square
                }
            }
            //if king moved
            if (piece1.Type == EPIECE.King)
            {//remove castling options
                if (piece1.Colour == ECOLOUR.Black)
                {
                    castle[2] = '\0';
                    castle[3] = '\0';
                }
                else
                {
                    castle[0] = '\0';
                    castle[1] = '\0';
                }
                if (Math.Abs(end.Column - start.Column) > 1) //if king moved 2 (castled)
                {
                    int rookRow = piece1.Colour switch
                    {
                        ECOLOUR.Black => 0,
                        ECOLOUR.White => 7,
                    };
                    int rookColumnStart = end.Column switch
                    {
                        2 => 0,
                        6 => 7,
                    };
                    int rookColumnEnd = end.Column switch
                    {
                        2 => 3,
                        6 => 5,
                    };
                    pieces[rookRow, rookColumnEnd] = pieces[rookRow, rookColumnStart];
                    pieces[rookRow, rookColumnStart] = Piece.Null;
                }
            }
            //if piece moved was pawn or square moved to had piece (piece was captured)
            if (piece1.Type == EPIECE.Pawn || piece2.Type != EPIECE.Null)
                halfm = 0; //reset half-move counter
            //else increment
            else
                halfm += 1;
            //increment full-move counter
            fullm += 1;

            //check for stalemate (and mate later)
            Dictionary<Pos, List<Pos>> d = AllMoves();
            if (d.Count == 0)
                winner = EWINNER.Draw;
        }

        internal bool PlayerMove(Pos start, Pos end)
        {
            Piece piece1 = pieces[start.Row, start.Column];

            if (piece1.Colour == turn)
            {
                if (LegalMoves(start).Contains(end)) /*check if piece can move to destination*/
                {
                    MovePiece(start, end);
                    return true;
                }
            }
            return false;
        }

        internal Dictionary<Pos, List<Pos>> AllMoves()
        {
            Dictionary<Pos, List<Pos>> d = new();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (pieces[i, j].Colour != ECOLOUR.Null)
                    {
                        Pos a = new(i, j);
                        List<Pos> l = LegalMoves(a);
                        if (l.Count > 0)
                            d.Add(a, l);
                    }
                }
            }
            return d;
        }
        internal Dictionary<Pos, List<Pos>> ColourMoves(ECOLOUR colour)
        {
            Dictionary<Pos, List<Pos>> d = new();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (pieces[i, j].Colour == colour)
                    {
                        Pos a = new( i, j );
                        List<Pos> l = LegalMoves(a);
                        if (l.Count > 0)
                            d.Add(a, l);
                    }
                }
            }
            return d;
        }

        internal void AIMove()
        {
            Dictionary<Pos, List<Pos>> pieceList = ColourMoves(turn);
            Random r = new();
            int i = r.Next(pieceList.Count);
            Pos startPos = pieceList.Keys.ToList()[i];
            List<Pos> moveList = pieceList.Values.ToList()[i];
            int j = r.Next(0, moveList.Count);
            Pos endPos = moveList[j];
            MovePiece(startPos, endPos);
        }
        internal List<Pos> LegalMoves(Pos pos)
        {
            List<Pos> l = new();

            Piece p = pieces[pos.Row, pos.Column];

            switch (p.Type)
            {
                case EPIECE.King:
                    l.AddRange(GetKingMoves(pos));
                    break;
                case EPIECE.Queen:
                    l.AddRange(GetStraightMoves(pos));
                    l.AddRange(GetDiagMoves(pos));
                    break;
                case EPIECE.Rook:
                    l.AddRange(GetStraightMoves(pos));
                    break;
                case EPIECE.Bishop:
                    l.AddRange(GetDiagMoves(pos));
                    break;
                case EPIECE.Knight:
                    l.AddRange(GetKnightMoves(pos));
                    break;
                case EPIECE.Pawn:
                    l.AddRange(GetPawnMoves(pos));
                    break;
            }

            return l;
        }

        private Piece PieceAt(Pos pos) //get piece at pos
        {
            return pieces[pos.Row,pos.Column];
        }
        private static bool PosInBounds( Pos pos) //check given pos is within boundaries of the board e.g. 0-7 row and column
        {
            if (pos.Row >= 0 && pos.Row <= 7 && pos.Column >= 0 && pos.Column <= 7)
                return true;
            else
                return false;
        }
        private List<Pos> GetLineDirectionMoves(Pos pos, int rowIncrement, int columnIncrement)
        {
            List<Pos> l = new();

            Piece pieceStart = PieceAt(pos);

            while (PosInBounds(pos))
            {
                pos.Row += rowIncrement;
                pos.Column += columnIncrement;
                if (PosInBounds(pos))
                {
                    Piece pieceEnd = PieceAt(pos);
                    if (pieceEnd.Colour != pieceStart.Colour)
                        l.Add(pos);
                    if (pieceEnd.Colour != ECOLOUR.Null)
                        break;
                }
            }

            return l;
        }
        private List<Pos> GetSingleMove(Pos pos, int rowIncrement, int columnIncrement)
        {
            List<Pos> l = new();

            Piece pieceStart = PieceAt(pos);
            pos.Row += rowIncrement;
            pos.Column += columnIncrement;
            if (PosInBounds(pos))
            {
                Piece pieceEnd = PieceAt(pos);
                if (pieceEnd.Colour != pieceStart.Colour)
                    l.Add(pos);
            }

            return l;
        }
        private List<Pos> GetPawnMoves(Pos pos, int directionMult)
        {
            List<Pos> l = new();

            Piece pieceStart = PieceAt(pos);
            int i = directionMult switch //todo single line algorithm to convert?
            {
                1 => 1,
                -1 => 6,
            };
            pos.Row += directionMult; //check forwards move
            if (PosInBounds(pos)) 
            {
                Piece pieceEnd = PieceAt(pos);
                if (pieceEnd.Colour == ECOLOUR.Null)
                    l.Add(pos);
            }
            if (pos.Row - directionMult == i) //check double forwards move
            {
                Pos pos2 = new(pos.Row + directionMult, pos.Column);

                if (PieceAt(pos2).Colour == ECOLOUR.Null && PieceAt(pos).Colour == ECOLOUR.Null) //check if squares are empty
                    l.Add(pos2);
            }
            pos.Column -= 1; //check left capture
            if (PosInBounds(pos))
            {
                Piece pieceEnd = PieceAt(pos);
                if (pieceEnd.Colour != ECOLOUR.Null && pieceEnd.Colour != pieceStart.Colour)
                    l.Add(pos);
                else if (pos.Equals(passant)) //also add if passant square
                    l.Add(pos);
            }
            pos.Column += 2; //check right capture
            if (PosInBounds(pos))
            {
                Piece pieceEnd = PieceAt(pos);
                if (pieceEnd.Colour != ECOLOUR.Null && pieceEnd.Colour != pieceStart.Colour)
                    l.Add(pos);
                else if (pos.Equals(passant)) //also add if passant square
                    l.Add(pos);
            }

            return l;
        }

        private List<Pos> GetStraightMoves(Pos pos)
        {
            List<Pos> l = new();

            l.AddRange(GetLineDirectionMoves(pos, -1, 0)); //up
            l.AddRange(GetLineDirectionMoves(pos, 1, 0)); //down
            l.AddRange(GetLineDirectionMoves(pos, 0, -1)); //left
            l.AddRange(GetLineDirectionMoves(pos, 0, 1)); //right

            return l;
        }
        private List<Pos> GetDiagMoves(Pos pos)
        {
            List<Pos> l = new();

            l.AddRange(GetLineDirectionMoves(pos, -1, -1)); //upleft
            l.AddRange(GetLineDirectionMoves(pos, -1, 1)); //upright
            l.AddRange(GetLineDirectionMoves(pos, 1, -1)); //downleft
            l.AddRange(GetLineDirectionMoves(pos, 1, 1)); //downright

            return l;
        }
        private List<Pos> GetKingMoves(Pos pos)
        {
            List<Pos> l = new();
            Piece pieceStart = PieceAt(pos);
            Dictionary<Pos, List<Pos>> enemyMoves = ColourMoves(pieceStart.ColourOpposite); //STACK OVERFLOW: ColourMoves calls GetKingMoves, however
            //kings can't move in range of each other so I do have to check that in some way

            for (int i = 0; i < 4; i++)//check castling
            {
                if (ColourFromChar(castle[i]) == pieceStart.Colour)//if can castle own colour king/queenside
                {
                    if (ToUpper(castle[i]) == 'K')
                    {//kingside
                        Pos p = pos; //check squares in between for pieces
                        p.Column += 1;
                        if (PieceAt(p).Colour == ECOLOUR.Null)
                            p.Column += 1;
                        if (PieceAt(p).Colour == ECOLOUR.Null)
                        {
                            if (!enemyMoves.Any(x => x.Value.Contains(p))) //see if move would not put king in check
                                l.Add(p);
                        }
                    }
                    else
                    {//queenside
                        Pos p = pos; //check squares in between for pieces
                        p.Column -= 1;
                        if (PieceAt(p).Colour == ECOLOUR.Null)
                            p.Column -= 1;
                        if (PieceAt(p).Colour == ECOLOUR.Null)
                            p.Column -= 1;
                        if (PieceAt(p).Colour == ECOLOUR.Null)
                        {
                            p.Column += 1;
                            if (!enemyMoves.Any(x => x.Value.Contains(p))) //see if move would not put king in check
                                l.Add(p);
                        }
                    }
                }
            }
            for (int i = -1; i < 2; i++) //get normal king moves
            {
                pos.Row += i;
                for (int j = -1; j < 2; j++)
                {
                    pos.Column += j;
                    if (PosInBounds(pos) && !(i == 0 && j == 0)) //skip square king is on
                    {
                        Piece pieceEnd = PieceAt(pos);
                        if (pieceEnd.Colour != pieceStart.Colour) //todo also see whether move is in check
                        {
                            if (!enemyMoves.Any(x => x.Value.Contains(pos))) //see if move would not put king in check
                                l.Add(pos);
                        }
                    }
                }
            }

            return l;
        }
        private List<Pos> GetKnightMoves(Pos pos)
        {
            List<Pos> l = new();

            l.AddRange(GetSingleMove(pos, -2, -1));
            l.AddRange(GetSingleMove(pos, -2, 1));
            l.AddRange(GetSingleMove(pos, -1, -2));
            l.AddRange(GetSingleMove(pos, -1, 2));
            l.AddRange(GetSingleMove(pos, 1, -2));
            l.AddRange(GetSingleMove(pos, 1, 2));
            l.AddRange(GetSingleMove(pos, 2, -1));
            l.AddRange(GetSingleMove(pos, 2, 1));

            return l;
        }
        private List<Pos> GetPawnMoves(Pos pos)
        {
            List<Pos> l = new();

            int directionMult = PawnDirectionMult(PieceAt(pos).Colour);
            l.AddRange(GetPawnMoves(pos, directionMult));

            return l;
        }
        private static int PawnDirectionMult(ECOLOUR c)
        {
            if (c == ECOLOUR.Black)
                return 1;//black
            else
                return -1;//white
        }
    }
}
