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
        public MovePlan GetNextMove(bool team) {
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
                        return new MovePlan() { 
                            StartRow = positionMove.Row, 
                            StartColumn = positionMove.Column, 
                            Move = move,
							Wins = 1
                        };
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
			return movePlans.OrderByDescending(p => p.Wins / (p.Loses + p.Incomplete + p.Wins)).FirstOrDefault();
        }

        public MovePlan GetMovePlans(bool team, Piece?[,] pieces, bool isOpponentTurn, int depth)
        {
            var opponent = !team;
            var finder = new LegalMoveFinder(pieces);
            var status = new GameStatus(pieces);
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
                    else if (newBoard.Winner == team && !isOpponentTurn)
                        return new MovePlan()
                        {
                            StartRow = positionMove.Row,
                            StartColumn = positionMove.Column,
                            Move = move,
                            Wins = 1
                        };
                    else if (depth == MovePlan.MAX_DEPTH)
                        ++incomplete;
					else
                    {
                        MovePlan plan = GetMovePlans(team, newPieces, !isOpponentTurn, depth + 1);
                        if (!isOpponentTurn && plan.Loses == 0 && plan.Incomplete == 0 && plan.Wins > 0)
                            return new MovePlan()
                            {
                                StartRow = positionMove.Row,
                                StartColumn = positionMove.Column,
                                Move = move,
                                Wins = plan.Wins
                            };
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