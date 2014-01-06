using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckersAI.Models
{
    public class LegalMoveFinder
    {
        private Piece?[,] _pieces; 
        private LegalMoveFinder() { }
        public const int MIN_POSITION = 0;
        public const int MAX_POSITION = 7;

        public LegalMoveFinder(Piece?[,] pieces)
        {
            if (pieces.Rank != 2 || pieces.GetLength(0) != MAX_POSITION + 1 || pieces.GetLength(1) != MAX_POSITION + 1)
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
            if (_pieces[row, column].HasValue)
            {
                var piece = _pieces[row, column].Value;
                var rowChange = PieceUtil.OnDownTeam(piece) ? 1 : -1;
                var directions = new List<MoveDirection>();
                directions.AddRange(GetNonJumpingDirection(row + rowChange, column - 1, ForwardLeft(piece)));
                directions.AddRange(GetNonJumpingDirection(row + rowChange, column + 1, ForwardRight(piece)));
                if (PieceUtil.IsKing(piece))
                {
                    directions.AddRange(GetNonJumpingDirection(row - rowChange, column - 1, BackwardLeft(piece)));
                    directions.AddRange(GetNonJumpingDirection(row - rowChange, column + 1, BackwardRight(piece)));
                }
                return directions.Select(d => new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = d, Jump = false } } }).ToList();
            }
            return new List<Move>();
        }

        private List<MoveDirection> GetNonJumpingDirection(int row, int column, MoveDirection direction) {
            if (row >= MIN_POSITION && row <= MAX_POSITION && column >= MIN_POSITION && column <= MAX_POSITION && _pieces[row, column] == null)
                return new List<MoveDirection>() { direction };
            else
                return new List<MoveDirection>();
        }

        private List<Move> GetJumpingMoves(int row, int column)
        {
            if (_pieces[row, column].HasValue)
            {
                var piece = _pieces[row, column].Value;
                var piecesClone = (Piece?[,])_pieces.Clone();
                piecesClone[row, column] = null;
                return GetJumpingSteps(row, column, piece, new List<MoveStep>(), piecesClone);
            }
            return new List<Move>();
        }

        private List<Move> GetJumpingSteps(int row, int column, Piece piece, List<MoveStep> moveSteps, Piece?[,] piecesClone)
        {
            var moves = new List<Move>();
            moves.AddRange( GetStepsInDirection(row, column, ForwardLeft(piece), piecesClone, piece, moveSteps) );
            moves.AddRange( GetStepsInDirection(row, column, ForwardRight(piece), piecesClone, piece, moveSteps) );
            if (PieceUtil.IsKing(piece))
            {
                moves.AddRange( GetStepsInDirection(row, column, BackwardLeft(piece), piecesClone, piece, moveSteps) );
                moves.AddRange( GetStepsInDirection(row, column, BackwardRight(piece), piecesClone, piece, moveSteps) );
            }
            return moves;
        }

        private List<Move> GetStepsInDirection(int row, int column, MoveDirection direction, Piece?[,] piecesClone, Piece piece, List<MoveStep> moveSteps)
        {
            var moves = new List<Move>();
            int jumpToRow, jumpToColumn, captureRow, captureColumn;
            GetRelativePositions(direction, row, column, out jumpToRow, out jumpToColumn, out captureRow, out captureColumn);
            if (jumpToRow >= MIN_POSITION && jumpToRow <= MAX_POSITION)
            {
                var isValidCaturePos = captureColumn >= MIN_POSITION && captureColumn <= MAX_POSITION;
                var capturePiece = isValidCaturePos ? piecesClone[captureRow, captureColumn] : null;
                var isValidJumpPos = jumpToColumn >= MIN_POSITION && jumpToColumn <= MAX_POSITION;
                var jumpPosition = isValidJumpPos ? piecesClone[jumpToRow, jumpToColumn] : null;
                var isOpponentPiece = capturePiece != null && PieceUtil.OnDownTeam(capturePiece.Value) != PieceUtil.OnDownTeam(piece);
                if (isValidJumpPos && isOpponentPiece && jumpPosition == null)
                {
                    var move = new Move() { Steps = moveSteps.ToList() };
                    move.Steps.Add(new MoveStep() { Direction = direction, Jump = true });
                    moves.Add(move);
                    var nextClone = (Piece?[,])piecesClone.Clone();
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
            return PieceUtil.OnDownTeam(piece) ? MoveDirection.DOWN_LEFT : MoveDirection.UP_LEFT;
        }

        private MoveDirection ForwardRight(Piece piece)
        {
            return PieceUtil.OnDownTeam(piece) ? MoveDirection.DOWN_RIGHT : MoveDirection.UP_RIGHT;
        }

        private MoveDirection BackwardLeft(Piece piece)
        {
            return PieceUtil.OnDownTeam(piece) ? MoveDirection.UP_LEFT : MoveDirection.DOWN_LEFT;
        }

        private MoveDirection BackwardRight(Piece piece)
        {
            return PieceUtil.OnDownTeam(piece) ? MoveDirection.UP_RIGHT : MoveDirection.DOWN_RIGHT;
        }

        public List<PositionMoves> GetLegalMoves(bool team)
        {
            var list = new List<PositionMoves>();
            for (var r = MIN_POSITION; r <= MAX_POSITION; ++r)
                for (var c = MIN_POSITION; c <= MAX_POSITION; ++c)
                {
                    var piece = _pieces[r, c];
                    if (piece != null && PieceUtil.OnDownTeam(piece.Value) == team)
                        list.Add(new PositionMoves()
                        {
                            Row = r,
                            Column = c,
                            Moves = GetLegalMoves(r, c)
                        });
                }
            return list;
        }
    }

    public struct PositionMoves
    {
        public int Row;
        public int Column;
        public List<Move> Moves;
    }

    public struct Move { 
        public List<MoveStep> Steps;
        public override string ToString() { return String.Join(", ", Steps.Select(s => s.ToString())); }
    }

    public struct MoveStep {
        public MoveDirection Direction;
        public bool Jump;
        public override string ToString() { return Direction.ToString() + " " + (Jump ? "Jump" : ""); }
    }

    public enum MoveDirection {
        UP_LEFT, UP_RIGHT, DOWN_LEFT, DOWN_RIGHT
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