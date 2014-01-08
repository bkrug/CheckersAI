using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckersAI.Models
{
	public class MovePlanner
	{
        private Piece?[,] _pieces;

		private MovePlanner() {}
        public MovePlanner(Piece?[,] pieces) {
            _pieces = pieces;
        }

		//TODO: A unit test will end up testing other methods as well as this one.
        public RecommendedMove GetNextMove(bool team) {
            var finder = new LegalMoveFinder(_pieces);
            var movePlans = new List<MovePlan>();
            var status = new GameStatus(_pieces);
            foreach (var positionMove in finder.GetLegalMoves(team))
            {
                foreach (var move in positionMove.Moves)
                {
                    var newPieces = status.CloneAndMove(positionMove.Row, positionMove.Column, move);
                    var newBoard = new GameStatus(newPieces);
                    if (newBoard.Winner == team)
                        return new RecommendedMove() { Row = positionMove.Row, Column = positionMove.Column, Move = move };
                    else
                    {
                        var plan = GetMovePlans(team, newPieces, true, 1);
                        plan.StartRow = positionMove.Row;
                        plan.StartColumn = positionMove.Column;
                        plan.Move = move;
                        movePlans.Add(plan);
                    }
                }
            }
            //TODO: Need a way to measure the length of a plan.
			var selected = movePlans.OrderBy(p => p.Loses == 0).ThenBy(p => p.Wins / p.Loses).First();
            return new RecommendedMove()
            {
                Row = selected.StartRow,
                Column = selected.StartColumn,
                Move = selected.Move
            };
        }

        public MovePlan GetMovePlans(bool team, Piece?[,] pieces, bool isOpponentTurn, int depth)
        {
            var opponent = !team;
            var finder = new LegalMoveFinder(pieces);
            var status = new GameStatus(_pieces);
			var loses = 0;
			var wins = 0;
			var incomplete = 0;
            foreach (var positionMove in finder.GetLegalMoves(isOpponentTurn ? opponent : team))
            {
                foreach (var move in positionMove.Moves)
                {
                    var newPieces = status.CloneAndMove(positionMove.Row, positionMove.Column, move);
                    var newBoard = new GameStatus(newPieces);
                    if (newBoard.Winner == opponent)
                        ++loses;
                    else if (newBoard.Winner == team)
                        ++wins;
                    else if (depth == MovePlan.MAX_DEPTH)
                        ++incomplete;
                    else
                    {
                        MovePlan plan = GetMovePlans(team, newPieces, !isOpponentTurn, depth + 1);
                        loses += plan.Loses;
                        wins += plan.Wins;
                        incomplete += plan.Incomplete;
                    }
                }
            }
            return new MovePlan() { Incomplete = incomplete, Wins = wins, Loses = loses };
        }
	}
}