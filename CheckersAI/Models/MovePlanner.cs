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
            var finder = new LegalMoveFinder(_pieces);
            var board = new GameStatus(_pieces);
            var movePlans = new List<MovePlan>();
            foreach (var positionMove in finder.GetLegalMoves(team))
            {
                foreach (var move in positionMove.Moves)
                {
                    var piecesClone = board.CloneAndMove(positionMove.Row, positionMove.Column, move);
                    var plan = GetMovePlans(piecesClone, _planDepth, Int32.MinValue, Int32.MaxValue, !team);
                    plan.StartRow = positionMove.Row;
                    plan.StartColumn = positionMove.Column;
                    plan.Move = move;
                    movePlans.Add(plan);
                }
            }
            return movePlans.OrderByDescending(p => p.Heuristic).FirstOrDefault();
        }

        public MovePlan GetMovePlans(Piece?[,] pieces, int depth, int alpha, int beta, bool isMaximizing)
        {
            var board = new GameStatus(pieces);
            if (board.Winner == isMaximizing)
                return new MovePlan() { Heuristic = -WinHeuristic };
            if (board.Winner == !isMaximizing)
                return new MovePlan() { Heuristic = WinHeuristic };
            if (depth == 0)
                return new MovePlan() { Heuristic = GetHeuristic(pieces) };
            if (isMaximizing)
            {
                var finder = new LegalMoveFinder(pieces);
                foreach (var positionMove in finder.GetLegalMoves(isMaximizing))
                {
                    foreach (var move in positionMove.Moves)
                    {
                        var piecesClone = board.CloneAndMove(positionMove.Row, positionMove.Column, move);
                        var potentialPlan = GetMovePlans(piecesClone, depth - 1, alpha, beta, !isMaximizing);
                        alpha = Math.Max(alpha, potentialPlan.Heuristic);
						if (beta <= alpha)
							return new MovePlan() { Heuristic = alpha };
                    }
                }
                return new MovePlan() { Heuristic = alpha };
            }
            else
            {
                var finder = new LegalMoveFinder(pieces);
                foreach (var positionMove in finder.GetLegalMoves(isMaximizing))
                {
                    foreach (var move in positionMove.Moves)
                    {
                        var piecesClone = board.CloneAndMove(positionMove.Row, positionMove.Column, move);
                        var potentialPlan = GetMovePlans(piecesClone, depth - 1, alpha, beta, !isMaximizing);
                        beta = Math.Min(beta, potentialPlan.Heuristic);
                        if (beta <= alpha)
                            return new MovePlan() { Heuristic = beta };
                    }
                }
                return new MovePlan() { Heuristic = beta };
            }
        }

        private int GetHeuristic(Piece?[,] pieces)
        {
			var heuristic = 0;
            for (var r = 0; r <= LegalMoveFinder.MAX_POSITION; ++r)
                for (var c = 0; c <= LegalMoveFinder.MAX_POSITION; ++c)
					if (pieces[r,c].HasValue)
                        heuristic += PieceUtil.OnDownTeam(pieces[r, c].Value) ? 1 : -1;
            return heuristic;
        }
	}
}