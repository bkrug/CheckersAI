using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckersAI.Models
{
    public class Piece
    {
        public bool IsKing { get; set; }
        public bool DownBoundTeam { get; set; }
    }

    //public class BoardState : IBoardState, ICloneable
    //{
    //    private Piece[,] _pieces;
    //    public BoardState(Piece[,] pieces) {
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