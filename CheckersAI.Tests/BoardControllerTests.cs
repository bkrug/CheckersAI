using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CheckersAI.Controllers;
using CheckersAI.Models;
using System.Web.Script.Serialization;
using Moq;
using System.Collections.Generic;

namespace CheckersAI.Tests
{
    [TestClass]
    public class BoardControllerTests

    {
        [TestMethod]
        public void ControllerBoard_MovePiece()
        {
            var board = new Mock<IGameBoard>();
            var controller = new BoardController();
            controller.Session[BoardController.BOARD_KEY] = board.Object;
            var move = new Move() { Steps = new List<MoveStep> { new MoveStep { Direction = MoveDirection.UP_RIGHT, Jump = true } } };
            var moveString = new JavaScriptSerializer().Serialize(move);
            controller.MovePiece(5, 4, moveString);
            board.Verify(m => m.MovePiece(5, 4, move), "A piece was moved by the calling procedure.");
        }
    }
}
