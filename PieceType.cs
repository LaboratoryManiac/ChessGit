using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    enum EMOVEMENTTYPE { Straight, Diagonal, Combined, LShape, Pawn} //rook, bishop, king/queen, knight, pawn
    internal class PieceType
    {
        internal EMOVEMENTTYPE MoveType;
        internal int MoveSize;

        /// <param name="c">Collides with pieces</param>
        /// <param name="mT">Movement type</param>
        /// <param name="mS">Movement distance (1 for pawn/king, 7 for non-knights, 3 for knights)</param>
        internal PieceType(EMOVEMENTTYPE mT, int mS)
        {
            MoveType = mT;
            MoveSize = mS;
        }
    }
}
