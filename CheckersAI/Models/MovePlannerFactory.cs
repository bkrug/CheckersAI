using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckersAI.Models
{
    public interface IMovePlannerFactory
    {
        IMovePlanner GetMovePlanner(IGameBoard board);
    }

    public class MovePlannerFactory : IMovePlannerFactory
    {
        private static MovePlannerFactory _instance = new MovePlannerFactory();
        private MovePlannerFactory() { }

        public static MovePlannerFactory Instance { get { return _instance; } }

        public IMovePlanner GetMovePlanner(IGameBoard board)
        {
            return new MovePlanner(board, 7);
        }
    }
}