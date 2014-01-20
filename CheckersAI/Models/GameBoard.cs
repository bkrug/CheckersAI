using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckersAI.Models
{
    public interface IGameBoard
    {
        void MovePiece(int row, int column, Move move);
        GameBoard CloneAndMove(int row, int column, Move move);
        GameBoard Clone();
        int UpPieces { get; }
        int DownPieces { get; }
        Piece? this[int r, int c] { get; set; }
        void Reset();
        bool? Winner { get; }
    }

    public class GameBoard : IGameBoard
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
                if (r < LegalMoveFinder.MIN_POSITION || r > LegalMoveFinder.MAX_POSITION || c < LegalMoveFinder.MIN_POSITION || c > LegalMoveFinder.MAX_POSITION)
                    throw new OffBoardException(r, c);
                if (c % 2 == r % 2)
                    throw new InvalidPositionException(r, c);
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

        public Piece?[][] PieceLayout {
            get
            {
                var array = new Piece?[8][];
                for (var r = 0; r <= LegalMoveFinder.MAX_POSITION; ++r)
                {
                    array[r] = new Piece?[8];
                    for (var c = 0; c <= LegalMoveFinder.MAX_POSITION; ++c)
                        array[r][c] = this[r, c];
                }
                return array;
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

        //public void Reset()
        //{
        //    this[7, 4] = Piece.DOWN_TEAM_KING;
        //    this[1, 2] = Piece.DOWN_TEAM;
        //    this[2, 3] = Piece.DOWN_TEAM;
        //    this[6, 1] = Piece.UP_TEAM;
        //    this[5, 0] = Piece.UP_TEAM;
        //    this[4, 1] = Piece.UP_TEAM;
        //    this[6, 7] = Piece.DOWN_TEAM;
        //    UpPieces = 3;
        //    DownPieces = 4;
        //}

        public void Reset()
        {
            for (var r = 0; r < 3; ++r)
                for (var c = (r + 1) % 2; c <= LegalMoveFinder.MAX_POSITION; c = c + 2)
                    this[r, c] = Piece.DOWN_TEAM;
            for (var r = 3; r < 5; ++r)
                for (var c = (r + 1) % 2; c <= LegalMoveFinder.MAX_POSITION; c = c + 2)
                    this[r, c] = null;
            for (var r = 5; r <= LegalMoveFinder.MAX_POSITION; ++r)
                for (var c = (r + 1) % 2; c <= LegalMoveFinder.MAX_POSITION; c = c + 2)
                    this[r, c] = Piece.UP_TEAM;
            UpPieces = 12;
            DownPieces = 12;
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