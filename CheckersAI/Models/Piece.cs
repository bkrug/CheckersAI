using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckersAI.Models
{
    public enum Piece
    {
        UP_TEAM = 0x00,
        UP_TEAM_KING = 0x01,
        DOWN_TEAM = 0x02,
        DOWN_TEAM_KING = 0x03
    }

    [Flags]
    public enum PieceFlag
    {
        KING = 0x01, DOWN_TEAM = 0x02
    }

    public class PieceUtil
    {
        public static bool IsKing(Piece piece) 
        {
            return ((int)piece & (int)PieceFlag.KING) > 0;
        }

        public static bool OnDownTeam(Piece piece)
        {
            return ((int)piece & (int)PieceFlag.DOWN_TEAM) > 0;
        }

        public static Piece King(Piece piece)
        {
            return (Piece)((int)piece | (int)PieceFlag.KING);
        }
    }

    //public class BoardState : IBoardState, ICloneable
    //{
    //    private Piece?[,] _pieces;
    //    public BoardState(Piece?[,] pieces) {
    //        _pieces = pieces;
    //    }
    //    public Piece GetPiece(int row, int column) {
    //        return _pieces[row, column];
    //    }
    //    public void MovePiece(int row1, int column1, Move move) {
    //        var piece = _pieces[row1, column1];
    //        _pieces[row1, column1] = null;
    //    }
    //    public object Clone() {
    //        return new object();
    //    }
    //}

    //public interface IBoardState
    //{
    //    Piece GetPiece(int row, int column);
    //    Piece MovePiece(int row, int column, Move move);
    //}
}