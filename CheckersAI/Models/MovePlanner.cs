using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckersAI.Models
{
	public class MovePlanner
	{
        private Piece?[,] _pieces;
        private int _planDepth;

        public static int WinHeuristic
        {
            get { return Int32.MaxValue; }
        }

		private MovePlanner() {}
        public MovePlanner(Piece?[,] pieces, int planDepth) {
            _pieces = pieces;
            _planDepth = planDepth;
        }

		//TODO: A unit test will end up testing other methods as well as this one.
        public MovePlan GetNextMove(bool team) {
            var board = new GameBoard(_pieces);
            var finder = new LegalMoveFinder(board);
            var movePlans = new List<MovePlan>();
            foreach (var positionMove in finder.GetLegalMoves(team))
            {
                foreach (var move in positionMove.Moves)
                {
                    var newBoard = board.CloneAndMove(positionMove.Row, positionMove.Column, move);
                    var plan = GetMovePlans(newBoard, _planDepth, Int32.MinValue, Int32.MaxValue, !team);
                    plan.StartRow = positionMove.Row;
                    plan.StartColumn = positionMove.Column;
                    plan.Move = move;
                    movePlans.Add(plan);
                }
            }
            return team ? movePlans.OrderByDescending(p => p.Heuristic).FirstOrDefault() 
						: movePlans.OrderBy(p => p.Heuristic).FirstOrDefault();
        }

        public MovePlan GetMovePlans(GameBoard board, int depth, int alpha, int beta, bool isMaximizing)
        {
            var winner = board.WinnerByElimination;
            if (winner == true)
                return new MovePlan() { Heuristic = WinHeuristic };
            if (winner == false)
                return new MovePlan() { Heuristic = -WinHeuristic };
            if (depth == 0)
                return new MovePlan() { Heuristic = GetHeuristic(board) };
            if (isMaximizing)
            {
                var finder = new LegalMoveFinder(board);
                foreach (var positionMove in finder.GetLegalMoves(isMaximizing))
                {
                    foreach (var move in positionMove.Moves)
                    {
                        var newBoard = board.CloneAndMove(positionMove.Row, positionMove.Column, move);
                        var potentialPlan = GetMovePlans(newBoard, depth - 1, alpha, beta, !isMaximizing);
                        alpha = Math.Max(alpha, potentialPlan.Heuristic);
						if (beta <= alpha)
							return new MovePlan() { Heuristic = alpha };
                    }
                }
                return new MovePlan() { Heuristic = alpha };
            }
            else
            {
                var finder = new LegalMoveFinder(board);
                foreach (var positionMove in finder.GetLegalMoves(isMaximizing))
                {
                    foreach (var move in positionMove.Moves)
                    {
                        var newBoard = board.CloneAndMove(positionMove.Row, positionMove.Column, move);
                        var potentialPlan = GetMovePlans(newBoard, depth - 1, alpha, beta, !isMaximizing);
                        beta = Math.Min(beta, potentialPlan.Heuristic);
                        if (beta <= alpha)
                            return new MovePlan() { Heuristic = beta };
                    }
                }
                return new MovePlan() { Heuristic = beta };
            }
        }

        private int GetHeuristic(GameBoard board)
        {
			var heuristic = board.DownPieces - board.UpPieces;
            return heuristic;
        }
	}
}