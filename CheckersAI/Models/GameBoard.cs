using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckersAI.Models
{
    public class GameBoard
    {
        private Piece?[,] _pieces;

        public int GetLenght(int dimension)
        {
            return _pieces.GetLength(dimension);
        }

        public Piece? this[int r, int c]
        {
            get { return _pieces[r, c]; }
            set
            {
                if (_pieces[r,c].HasValue)
                    if (PieceUtil.OnDownTeam(_pieces[r,c].Value))
                        --DownPieces;
                    else
                        --UpPieces;
                _pieces[r, c] = value;
                if (_pieces[r, c].HasValue)
                    if (PieceUtil.OnDownTeam(_pieces[r, c].Value))
                        ++DownPieces;
                    else
                        ++UpPieces;
            }
        }

        public int UpPieces { get; private set; }
        public int DownPieces { get; private set; }

        public GameBoard()
        {
            _pieces = new Piece?[8, 8];
            UpPieces = 0;
            DownPieces = 0;
        }

        public GameBoard(Piece?[,] pieces) : this (pieces, true) 
        {
        }

        private GameBoard(Piece?[,] pieces, bool countPieces)
        {
            if (pieces.GetLength(0) != LegalMoveFinder.MAX_POSITION + 1 || pieces.GetLength(1) != LegalMoveFinder.MAX_POSITION + 1)
                throw new InvalidBoardException(pieces.GetLength(0), pieces.GetLength(1));
            _pieces = (Piece?[,])pieces.Clone();
            if (countPieces)
                CountUpAndDownPieces();
        }

        private void CountUpAndDownPieces()
        {
            UpPieces = 0;
            DownPieces = 0;
            for (var r = LegalMoveFinder.MIN_POSITION; r <= LegalMoveFinder.MAX_POSITION; ++r)
                for (var c = LegalMoveFinder.MIN_POSITION; c <= LegalMoveFinder.MAX_POSITION; ++c)
                    if (this[r,c].HasValue)
                        if (PieceUtil.OnDownTeam(this[r, c].Value))
                            ++DownPieces;
                        else
                            ++UpPieces;                    
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
                if (UpPieces == 0 && DownPieces > 0)
                    return true;
                if (DownPieces == 0 && UpPieces > 0)
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
                var moveFinder = new LegalMoveFinder(this);
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
            MovePiece(this, row, column, move);
        }

        private void MovePiece(GameBoard pieces, int row, int column, Move move)
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

        public GameBoard Clone()
        {
            var newBoard = new GameBoard(_pieces, false);
            newBoard.UpPieces = UpPieces;
            newBoard.DownPieces = DownPieces;
            return newBoard;
        }

        public GameBoard CloneAndMove(int row, int column, Move move)
        {
            var newBoard = Clone();
            MovePiece(newBoard, row, column, move);
            return newBoard;
        }
    }

    public class InvalidBoardException : ApplicationException
    {
        private InvalidBoardException() { }
        public InvalidBoardException(int row, int column) : base("The board side of " + row + "x" + column + " is invalid. Must be 8x8.") { }
    }

    public class OffBoardException : ApplicationException 
    {
        private OffBoardException() { }
        public OffBoardException(int row, int column) : base("The position " + row + ", " + column + " is off the board.") { }
    }

    public class InvalidPositionException : ApplicationException
    {
        private InvalidPositionException() { }
        public InvalidPositionException(int row, int column) : base("The position " + row + ", " + column + " is invalid.") { }
    }
}