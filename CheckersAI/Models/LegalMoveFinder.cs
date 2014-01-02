using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckersAI.Models
{
    public class LegalMoveFinder
    {
        private Piece[,] _pieces; 
        private LegalMoveFinder() { }
        public LegalMoveFinder(Piece[,] pieces) {
            if (pieces.Rank != 2 || pieces.GetLength(0) != BoardPosition.MAX_POSITION + 1 || pieces.GetLength(1) != BoardPosition.MAX_POSITION + 1)
                throw new ApplicationException("Array of pieces must be 8x8.");
            _pieces = pieces;
        }

        public List<Move> GetLegalMoves(int row, int column)
        {
            return GetNonJumpingMoves(row, column)
                .Union(GetJumpingMoves(row, column))
                .ToList();
        }

        private List<Move> GetNonJumpingMoves(int row, int column)
        {
            var piece = _pieces[row, column];
            var rowChange = piece.DownBoundSide ? 1 : -1;
            var directions = new List<MoveDirection>();
            if (_pieces[row + rowChange, column - 1] == null)
                directions.Add(ForwardLeft(piece));
            if (_pieces[row + rowChange, column + 1] == null)
                directions.Add(ForwardRight(piece));
            if (piece.IsKing)
            {
                if (_pieces[row - rowChange, column - 1] == null)
                    directions.Add(BackwardLeft(piece));
                if (_pieces[row - rowChange, column + 1] == null)
                    directions.Add(BackwardRight(piece));
            }
            return directions.Select(d => new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = d, Jump = false } } }).ToList();
        }

        private List<Move> GetJumpingMoves(int row, int column)
        {
            var piece = _pieces[row, column];
            var piecesClone = (Piece[,])_pieces.Clone();
            piecesClone[row, column] = null;
            return GetJumpingSteps(row, column, piece, new List<MoveStep>(), piecesClone);
        }

        private List<Move> GetJumpingSteps(int row, int column, Piece piece, List<MoveStep> moveSteps, Piece[,] piecesClone)
        {
            var moves = new List<Move>();
            moves.AddRange( GetStepsInDirection(row, column, ForwardLeft(piece), piecesClone, piece, moveSteps) );
            moves.AddRange( GetStepsInDirection(row, column, ForwardRight(piece), piecesClone, piece, moveSteps) );
            if (piece.IsKing)
            {
                moves.AddRange( GetStepsInDirection(row, column, BackwardLeft(piece), piecesClone, piece, moveSteps) );
                moves.AddRange( GetStepsInDirection(row, column, BackwardRight(piece), piecesClone, piece, moveSteps) );
            }
            return moves;
        }

        private List<Move> GetStepsInDirection(int row, int column, MoveDirection direction, Piece[,] piecesClone, Piece piece, List<MoveStep> moveSteps)
        {
            var moves = new List<Move>();
            int jumpToRow, jumpToColumn, captureRow, captureColumn;
            GetRelativePositions(direction, row, column, out jumpToRow, out jumpToColumn, out captureRow, out captureColumn);
            if (jumpToRow >= BoardPosition.MIN_POSITION && jumpToRow <= BoardPosition.MAX_POSITION)
            {
                var isValidCaturePos = captureColumn >= BoardPosition.MIN_POSITION && captureColumn <= BoardPosition.MAX_POSITION;
                var capturePiece = isValidCaturePos ? piecesClone[captureRow, captureColumn] : null;
                var isValidJumpPos = jumpToColumn >= BoardPosition.MIN_POSITION && jumpToColumn <= BoardPosition.MAX_POSITION;
                var jumpPosition = isValidJumpPos ? piecesClone[jumpToRow, jumpToColumn] : null;
                var isPieceOpponent = capturePiece != null && capturePiece.DownBoundSide != piece.DownBoundSide;
                if (isValidJumpPos && isPieceOpponent && jumpPosition == null)
                {
                    var move = new Move() { Steps = moveSteps.ToList() };
                    move.Steps.Add(new MoveStep() { Direction = direction, Jump = true });
                    moves.Add(move);
                    var nextClone = (Piece[,])piecesClone.Clone();
                    nextClone[captureRow, captureColumn] = null;
                    moves.AddRange(GetJumpingSteps(jumpToRow, jumpToColumn, piece, move.Steps, nextClone));
                    return moves;
                }
            }
            return moves;
        }

        private void GetRelativePositions(MoveDirection direction, int currentRow, int currentColumn, out int jumpToRow, out int jumpToColumn, out int captureRow, out int captureColumn)
        {
            if (direction == MoveDirection.UP_LEFT || direction == MoveDirection.UP_RIGHT)
            {
                captureRow = currentRow - 1;
                jumpToRow = currentRow - 2;
            }
            else
            {
                captureRow = currentRow + 1;
                jumpToRow = currentRow + 2;
            }
            if (direction == MoveDirection.UP_LEFT || direction == MoveDirection.DOWN_LEFT)
            {
                captureColumn = currentColumn - 1;
                jumpToColumn = currentColumn - 2;
            }
            else
            {
                captureColumn = currentColumn + 1;
                jumpToColumn = currentColumn + 2;
            }
        }

        private MoveDirection ForwardLeft(Piece piece)
        {
            return piece.DownBoundSide ? MoveDirection.DOWN_LEFT : MoveDirection.UP_LEFT;
        }

        private MoveDirection ForwardRight(Piece piece)
        {
            return piece.DownBoundSide ? MoveDirection.DOWN_RIGHT : MoveDirection.UP_RIGHT;
        }

        private MoveDirection BackwardLeft(Piece piece)
        {
            return piece.DownBoundSide ? MoveDirection.UP_LEFT : MoveDirection.DOWN_LEFT;
        }

        private MoveDirection BackwardRight(Piece piece)
        {
            return piece.DownBoundSide ? MoveDirection.UP_RIGHT : MoveDirection.DOWN_RIGHT;
        }
    }

    public struct Move { 
        public List<MoveStep> Steps;
        public string ToString() { return String.Join(", ", Steps.Select(s => s.ToString())); }
    }

    public struct MoveStep {
        public MoveDirection Direction;
        public bool Jump;
        public string ToString() { return Direction.ToString() + " " + (Jump ? "Jump" : ""); }
    }

    public enum MoveDirection {
        UP_LEFT, UP_RIGHT, DOWN_LEFT, DOWN_RIGHT
    }

    public class BoardPosition
    {
        public int Row { get; private set; }
        public int Column { get; private set; }
        public const int MIN_POSITION = 0;
        public const int MAX_POSITION = 7;

        private BoardPosition() { }
        public BoardPosition(int row, int column) 
        {
            if (row < MIN_POSITION || column < MIN_POSITION || row > MAX_POSITION || column > MAX_POSITION)
                throw new OffBoardException(row, column);
            if (row % 2 == column % 2)
                throw new InvalidPositionException(row, column);
            Row = row;
            Column = column;
        }
    }

    public class OffBoardException : ApplicationException {
        private OffBoardException() { }
        public OffBoardException(int row, int column) : base("The position " + row + ", " + column + " is off the board.") { }
    }

    public class InvalidPositionException : ApplicationException
    {
        private InvalidPositionException() { }
        public InvalidPositionException(int row, int column) : base("The position " + row + ", " + column + " is invalid.") { }
    }
}