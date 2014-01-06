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
        public MovePlanner(Piece?[,] pieces) { }

        public Move GetNextMove(bool team) {
            var finder = new LegalMoveFinder(_pieces);
            var movePlans = new List<MovePlan>();
            //TODO: Add method to LegalMoveFinder that will list all moves for a team
            foreach (var move in finder.GetLegalMoves(team))
            {
                //TODO: Add method to BoardStatus that will move a single piece and return a new BoardStatus object.
                //If move is clear win or clear lose, create a new move plan around it and add it to the list
                //otherwise, make a recursive call to find out the changes of a win or lose.
            }
            //TODO: Need a way to measure the length of a plan.
            return movePlans.OrderBy(p => p.Loses == 0).ThenBy(p => p.Wins / p.Loses).First().Move;
        }
	}
}