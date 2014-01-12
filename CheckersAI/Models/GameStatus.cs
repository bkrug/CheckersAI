using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckersAI.Models
{
    public class GameStatus
    {
        private Piece?[,] _pieces;

        public GameStatus(Piece?[,] pieces)
        {
            _pieces = pieces;
        }

        public bool? Winner
        {
            get
            {
                return WinnerByElimination.HasValue
                    ? WinnerByElimination
                    : WinnerByGridlock.HasValue ? WinnerByGridlock : null;
            }
        }

        public bool? WinnerByElimination
        {
            get
            {
                int downPieces = 0;
                int upPieces = 0;
                int r = 0;
                int c = 0;
                while ((downPieces == 0 || upPieces == 0) && r <= LegalMoveFinder.MAX_POSITION)
                {
                    var piece = _pieces[r, c++];
                    if (piece != null)
                        if (PieceUtil.OnDownTeam(piece.Value))
                            ++downPieces;
                        else
                            ++upPieces;
                    if (c > LegalMoveFinder.MAX_POSITION)
                    {
                        c = 0;
                        ++r;
                    }
                }
                if (upPieces == 0 && downPieces > 0)
                    return true;
                if (downPieces == 0 && upPieces > 0)
                    return false;
                return null;
            }
        }

        //SUSPECT: This is testing LegalMoveFinder() as well as one method.
        public bool? WinnerByGridlock
        {
            get
            {
                int downMoves = 0;
                int upMoves = 0;
                int r = 0;
                int c = 0;
                var moveFinder = new LegalMoveFinder(_pieces);
                while ((downMoves == 0 || upMoves == 0) && r <= LegalMoveFinder.MAX_POSITION)
                {
                    var piece = _pieces[r, c];
                    if (piece != null)
                        if (PieceUtil.OnDownTeam(piece.Value))
                            downMoves += moveFinder.GetLegalMoves(r, c).Count();
                        else
                            upMoves += moveFinder.GetLegalMoves(r, c).Count();
                    ++c;
                    if (c > LegalMoveFinder.MAX_POSITION)
                    {
                        c = 0;
                        ++r;
                    }
                }
                if (upMoves == 0 && downMoves > 0)
                    return true;
                if (downMoves == 0 && upMoves > 0)
                    return false;
                return null;
            }
        }

        public void MovePiece(int row, int column, Move move)
        {
            MovePiece(ref _pieces, row, column, move);
        }

        private void MovePiece(ref Piece?[,] pieces, int row, int column, Move move)
        {
            foreach(var step in move.Steps) {
                var oldRow = row;
                var oldColumn = column;
                var moveVertical = step.Direction == MoveDirection.UP_RIGHT || step.Direction == MoveDirection.UP_LEFT ? -1 : 1;
                var moveHorizontal = step.Direction == MoveDirection.UP_LEFT || step.Direction == MoveDirection.DOWN_LEFT ? -1 : 1;
                if (step.Jump)
                {
                    pieces[row + moveVertical, column + moveHorizontal] = null;
                    moveVertical *= 2;
                    moveHorizontal *= 2;
                }
                row += moveVertical;
                column += moveHorizontal;
                pieces[row, column] = pieces[oldRow, oldColumn];
                if (pieces[row, column].HasValue && (row == LegalMoveFinder.MIN_POSITION || row == LegalMoveFinder.MAX_POSITION))
                    pieces[row, column] = PieceUtil.King(pieces[row, column].Value);
                pieces[oldRow, oldColumn] = null;
            }
        }

        public Piece?[,] CloneAndMove(int row, int column, Move move)
        {
            var clonedPieces = (Piece?[,])_pieces.Clone();
            MovePiece(ref clonedPieces, row, column, move);
            return clonedPieces;
        }
    }
}