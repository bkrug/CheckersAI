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
            return GetNonJumpingMove(row, column)
                .Union(GetJumpingMove(row, column))
                .ToList();
        }

        private List<Move> GetNonJumpingMove(int row, int column)
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

        private List<Move> GetJumpingMove(int row, int column)
        {
            var piece = _pieces[row, column];
            var rowChange = piece.DownBoundSide ? 1 : -1;
            var piecesClone = (Piece[,])_pieces.Clone();
            piecesClone[row, column] = null;
            return GetJumpingSteps(row, column, piece, rowChange, new List<MoveStep>(), piecesClone);
        }

        private List<Move> GetJumpingSteps(int row, int column, Piece piece, int rowChange, List<MoveStep> moveSteps, Piece[,] piecesClone)
        {
            var moves = new List<Move>();
            var nextJumpRow = row + rowChange * 2;
            var nextRow = row + rowChange;
            var leftJumpColumn = column - 2;
            var rightJumpColumn = column + 2;
            if (nextRow >= BoardPosition.MIN_POSITION && nextRow <= BoardPosition.MAX_POSITION && nextJumpRow >= BoardPosition.MIN_POSITION && nextJumpRow <= BoardPosition.MAX_POSITION)
            {
                var forwardLeftPiece = column - 1 < BoardPosition.MIN_POSITION ? null : piecesClone[nextRow, column - 1];
                var forwardRightPiece = column + 1 > BoardPosition.MAX_POSITION ? null : piecesClone[nextRow, column + 1];
                var forwardLeftTwice = leftJumpColumn < BoardPosition.MIN_POSITION ? null : piecesClone[nextJumpRow, leftJumpColumn];
                var forwardRightTwice = rightJumpColumn > BoardPosition.MAX_POSITION ? null : piecesClone[nextJumpRow, rightJumpColumn];
                if (leftJumpColumn >= BoardPosition.MIN_POSITION && forwardLeftPiece != null && forwardLeftPiece.DownBoundSide != piece.DownBoundSide && forwardLeftTwice == null)
                {
                    var move = new Move() { Steps = moveSteps.ToList() };
                    move.Steps.Add(new MoveStep() { Direction = ForwardLeft(piece), Jump = true });
                    moves.Add(move);
                    var nextClone = (Piece[,])piecesClone.Clone();
                    nextClone[nextRow, column - 1] = null;
                    moves.AddRange(GetJumpingSteps(nextJumpRow, leftJumpColumn, piece, rowChange, move.Steps, nextClone));
                }
                if (rightJumpColumn <= BoardPosition.MAX_POSITION && forwardRightPiece != null && forwardRightPiece.DownBoundSide != piece.DownBoundSide && forwardRightTwice == null)
                {
                    var move = new Move() { Steps = moveSteps.ToList() };
                    move.Steps.Add(new MoveStep() { Direction = ForwardRight(piece), Jump = true });
                    moves.Add(move);
                    var nextClone = (Piece[,])piecesClone.Clone();
                    nextClone[nextRow, column + 1] = null;
                    moves.AddRange(GetJumpingSteps(nextJumpRow, rightJumpColumn, piece, rowChange, move.Steps, nextClone));
                }
            }
            if (piece.IsKing)
            {
                var prevJumpRow = row - rowChange * 2;
                var prevRow = row - rowChange;
                if (prevJumpRow < BoardPosition.MIN_POSITION || prevJumpRow > BoardPosition.MAX_POSITION)
                    return moves;
                var backwardLeftPiece = column - 1 < BoardPosition.MIN_POSITION ? null : piecesClone[prevRow, column - 1];
                var backwardRightPiece = column + 1 > BoardPosition.MAX_POSITION ? null : piecesClone[prevRow, column + 1];
                var backwardLeftTwice = leftJumpColumn < BoardPosition.MIN_POSITION ? null : piecesClone[prevJumpRow, leftJumpColumn];
                var backwardRightTwice = rightJumpColumn > BoardPosition.MAX_POSITION ? null : piecesClone[prevJumpRow, rightJumpColumn];
                if (leftJumpColumn >= BoardPosition.MIN_POSITION && backwardLeftPiece != null && backwardLeftPiece.DownBoundSide != piece.DownBoundSide && backwardLeftTwice == null)
                {
                    var move = new Move() { Steps = moveSteps.ToList() };
                    move.Steps.Add(new MoveStep() { Direction = BackwardLeft(piece), Jump = true });
                    moves.Add(move);
                    var nextClone = (Piece[,])piecesClone.Clone();
                    nextClone[prevRow, column - 1] = null;
                    moves.AddRange(GetJumpingSteps(prevJumpRow, leftJumpColumn, piece, rowChange, move.Steps, nextClone));
                }
                if (rightJumpColumn <= BoardPosition.MAX_POSITION && backwardRightPiece != null && backwardRightPiece.DownBoundSide != piece.DownBoundSide && backwardRightTwice == null)
                {
                    var move = new Move() { Steps = moveSteps.ToList() };
                    move.Steps.Add(new MoveStep() { Direction = BackwardRight(piece), Jump = true });
                    moves.Add(move);
                    var nextClone = (Piece[,])piecesClone.Clone();
                    nextClone[prevRow, column + 1] = null;
                    moves.AddRange(GetJumpingSteps(prevJumpRow, rightJumpColumn, piece, rowChange, move.Steps, nextClone));
                }
            }
            return moves;
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