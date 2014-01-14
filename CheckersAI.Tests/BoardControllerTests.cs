using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CheckersAI.Controllers;
using CheckersAI.Models;
using System.Web.Script.Serialization;
using Moq;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CheckersAI.Tests
{
    [TestClass]
    public class BoardControllerTests
    {
        public class MockSessionWrapper : ISessionWrapper
        {
            private Dictionary<string, object> _session = new Dictionary<string,object>();

            public object this[string index]
            {
                get { return _session[index]; }
                set
                {
                    _session[index] = value;
                }
            }
        }

        public class MockMovePlannerFactory : IMovePlannerFactory
        {
            private IMovePlanner _planner;

            public MockMovePlannerFactory(IMovePlanner planner) {
                _planner = planner;
            }

            public IMovePlanner GetMovePlanner(IGameBoard board) {
                return _planner;
            }
        }

        [TestMethod]
        public void ControllerBoard_MovePiece()
        {
            var board = new Mock<IGameBoard>();
            var planner = new Mock<IMovePlanner>();
            var session = new MockSessionWrapper();
            var controller = new BoardController(session, board.Object, new MockMovePlannerFactory(planner.Object));
            var move = new Move() { Steps = new List<MoveStep> { new MoveStep { Direction = MoveDirection.UP_RIGHT, Jump = true } } };
            var moveString = new JavaScriptSerializer().Serialize(move);
            var actionResult = controller.MovePiece(5, 4, moveString);
            board.Verify(m => m.MovePiece(5, 4, It.IsAny<Move>()), "A piece was moved by the calling procedure.");
            Assert.IsTrue(actionResult is ActionResult, "returned value is a type of ActionResult");
        }

        [TestMethod]
        public void ControllerBoard_GetComputerMove()
        {
            var team = true;
            var board = new Mock<IGameBoard>();
            var planner = new Mock<IMovePlanner>();
            var session = new MockSessionWrapper();
            session[BoardController.TEAM_KEY] = team;
            var controller = new BoardController(session, board.Object, new MockMovePlannerFactory(planner.Object));
            var actionResult = controller.GetComputerMove();
            planner.Verify(m => m.GetNextMove(!team), "The next computer move was requested.");
            Assert.IsTrue(actionResult is ActionResult, "returned value is a type of ActionResult");
        }

        [TestMethod]
        public void ControllerBoard_StartGame()
        {
            var team = true;
            var board = new Mock<IGameBoard>();
            var planner = new Mock<IMovePlanner>();
            var session = new MockSessionWrapper();
            var controller = new BoardController(session, board.Object, new MockMovePlannerFactory(planner.Object));
            var actionResult = controller.StartGame(team);
            board.Verify(m => m.Reset(), "Reset function was called.");
            Assert.AreEqual(session[BoardController.TEAM_KEY], team, "Game created with the correct team.");
            Assert.IsTrue(actionResult is ActionResult, "returned value is a type of ActionResult");
        }
    }
}
