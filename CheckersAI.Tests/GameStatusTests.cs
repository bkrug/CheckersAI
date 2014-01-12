using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CheckersAI.Models;
using System.Collections.Generic;

namespace CheckersAI.Tests
{
    [TestClass]
    public class GameStatusTests
    {
        [TestMethod]
        public void Status_WinnerByElimination()
        {
            {
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.DOWN_TEAM;
                pieces[5, 6] = Piece.DOWN_TEAM;
                pieces[2, 1] = Piece.DOWN_TEAM;
                var status1 = new GameStatus(pieces);
                Assert.AreEqual(true, status1.WinnerByElimination, "DownBoundTeam is calculated as winner.");
                pieces[7, 2] = Piece.UP_TEAM;
                Assert.AreEqual(null, status1.WinnerByElimination, "No team is calculated as winner.");
            }
            {
                var pieces = new Piece?[8, 8];
                var status1 = new GameStatus(pieces);
                pieces[4, 3] = Piece.UP_TEAM;
                pieces[5, 6] = Piece.UP_TEAM;
                pieces[2, 1] = Piece.UP_TEAM;
                Assert.AreEqual(false, status1.WinnerByElimination, "DownBoundTeam is calculated as loser.");
            }
            {
                var pieces = new Piece?[8, 8];
                var status1 = new GameStatus(pieces);
                pieces[2, 4] = Piece.DOWN_TEAM;
                pieces[3, 1] = Piece.DOWN_TEAM;
                pieces[5, 7] = Piece.UP_TEAM;
                Assert.AreEqual(null, status1.WinnerByElimination, "No team is winner.");
            }
        }

        [TestMethod]
        public void Status_WinnerByDeadlock()
        {
            {
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.DOWN_TEAM;
                pieces[5, 2] = Piece.UP_TEAM;
                pieces[5, 4] = Piece.UP_TEAM;
                pieces[6, 1] = Piece.UP_TEAM;
                pieces[6, 5] = Piece.UP_TEAM;
                var status1 = new GameStatus(pieces);
                Assert.AreEqual(false, status1.WinnerByGridlock, "UpBoundTeam is calculated as winner.");
                pieces[1, 2] = Piece.DOWN_TEAM;
                Assert.AreEqual(null, status1.WinnerByElimination, "No team is calculated as winner.");
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[6, 3] = Piece.UP_TEAM;
                pieces[5, 2] = Piece.DOWN_TEAM;
                pieces[5, 4] = Piece.DOWN_TEAM;
                pieces[4, 1] = Piece.DOWN_TEAM;
                pieces[4, 5] = Piece.DOWN_TEAM;
                var status1 = new GameStatus(pieces);
                Assert.AreEqual(true, status1.WinnerByGridlock, "UpBoundTeam is calculated as winner.");
            }
            {
                var pieces = new Piece?[8, 8];
                var status1 = new GameStatus(pieces);
                pieces[2, 4] = Piece.DOWN_TEAM;
                pieces[3, 1] = Piece.DOWN_TEAM;
                pieces[5, 7] = Piece.UP_TEAM;
                Assert.AreEqual(null, status1.WinnerByGridlock, "No team is winner.");
            }
        }

        [TestMethod]
        public void Status_MovePiece()
        {
            {
                var pieces = new Piece?[8, 8];
                //var originalBoard = (Piece?[,])pieces.Clone();
                pieces[4, 3] = Piece.DOWN_TEAM;
                pieces[4, 5] = Piece.DOWN_TEAM;
                pieces[3, 2] = Piece.UP_TEAM;
                pieces[3, 4] = Piece.UP_TEAM;
                pieces[6, 3] = Piece.DOWN_TEAM;
                pieces[1, 2] = Piece.UP_TEAM;
                var status = new GameStatus(pieces);
                {
                    var expectedBoard = (Piece?[,])pieces.Clone();
                    expectedBoard[5, 2] = expectedBoard[4, 3];
                    expectedBoard[4, 3] = null;
                    status.MovePiece(4, 3, new Move() { Steps = new List<MoveStep> { new MoveStep() { Direction = MoveDirection.DOWN_LEFT } } });
                    CompareBoards(expectedBoard, pieces, "Piece moved down-left.");
                }
                {
                    var expectedBoard = (Piece?[,])pieces.Clone();
                    expectedBoard[5, 6] = expectedBoard[4, 5];
                    expectedBoard[4, 5] = null;
                    status.MovePiece(4, 5, new Move() { Steps = new List<MoveStep> { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT } } });
                    CompareBoards(expectedBoard, pieces, "Piece moved down-right.");
                }
                {
                    var expectedBoard = (Piece?[,])pieces.Clone();
                    expectedBoard[2, 1] = expectedBoard[3, 2];
                    expectedBoard[3, 2] = null;
                    status.MovePiece(3, 2, new Move() { Steps = new List<MoveStep> { new MoveStep() { Direction = MoveDirection.UP_LEFT } } });
                    CompareBoards(expectedBoard, pieces, "Piece moved up-left");
                }
                {
                    var expectedBoard = (Piece?[,])pieces.Clone();
                    expectedBoard[2, 5] = expectedBoard[3, 4];
                    expectedBoard[3, 4] = null;
                    status.MovePiece(3, 4, new Move() { Steps = new List<MoveStep> { new MoveStep() { Direction = MoveDirection.UP_RIGHT } } });
                    CompareBoards(expectedBoard, pieces, "Piece moved up-right.");
                }
                {
                    var expectedBoard = (Piece?[,])pieces.Clone();
                    expectedBoard[7, 4] = PieceUtil.King(expectedBoard[6, 3].Value);
                    expectedBoard[6, 3] = null;
                    status.MovePiece(6, 3, new Move() { Steps = new List<MoveStep> { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT } } });
                    CompareBoards(expectedBoard, pieces, "Piece moved down-right and kinged.");
                }
                {
                    var expectedBoard = (Piece?[,])pieces.Clone();
                    expectedBoard[0, 1] = PieceUtil.King(expectedBoard[1, 2].Value);
                    expectedBoard[1, 2] = null;
                    status.MovePiece(1, 2, new Move() { Steps = new List<MoveStep> { new MoveStep() { Direction = MoveDirection.UP_LEFT } } });
                    CompareBoards(expectedBoard, pieces, "Piece moved up-left and kinged.");
                }
            }
        }

        [TestMethod]
        public void Status_MovePiece2()
        {
            {
                var pieces = new Piece?[8, 8];
                pieces[0, 3] = Piece.DOWN_TEAM;
                pieces[1, 4] = Piece.UP_TEAM;
                pieces[3, 4] = Piece.UP_TEAM;
                pieces[7, 0] = Piece.UP_TEAM;
                pieces[6, 1] = Piece.DOWN_TEAM;
                pieces[7, 2] = Piece.UP_TEAM;
                var status = new GameStatus(pieces);
                {
                    var expectedBoard = (Piece?[,])pieces.Clone();
                    expectedBoard[5, 2] = expectedBoard[7, 0];
                    expectedBoard[7, 0] = null;
                    expectedBoard[6, 1] = null;
                    status.MovePiece(7, 0, new Move()
                    {
                        Steps = new List<MoveStep> { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = true } }
                    });
                    CompareBoards(expectedBoard, pieces, "Captured one piece.");
                }
                {
                    var expectedBoard = (Piece?[,])pieces.Clone();
                    expectedBoard[4, 3] = expectedBoard[0, 3];
                    expectedBoard[0, 3] = null;
                    expectedBoard[1, 4] = null;
                    expectedBoard[3, 4] = null;
                    status.MovePiece(0, 3, new Move()
                    {
                        Steps = new List<MoveStep> { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true },
                                                     new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = true } }
                    });
                    CompareBoards(expectedBoard, pieces, "Captured two pieces.");
                }
            }
        }

        private void CompareBoards(Piece?[,] expected, Piece?[,] actual, string message)
        {
            for (var r = 0; r < 8; ++r)
                for (var c = 0; c < 8; ++c) 
                    if (expected[r, c] != actual[r, c])
                        Assert.Fail(message);
            Assert.IsTrue(true, message);
        }

        [TestMethod]
        public void Status_CloneBoardAndMove() {
            var expected = new Piece?[8, 8];
            expected[0, 3] = Piece.DOWN_TEAM;
            expected[1, 4] = Piece.UP_TEAM;
            expected[3, 4] = Piece.UP_TEAM;
            expected[7, 0] = Piece.UP_TEAM;
            expected[6, 1] = Piece.DOWN_TEAM;
            expected[7, 2] = Piece.UP_TEAM;
            var status = new GameStatus(expected);
            var clonedPieces = (Piece?[,])expected.Clone();
            var status1 = new GameStatus(clonedPieces);
            var move = new Move()
            {
                Steps = new List<MoveStep> { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true },
                                             new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = true } }
            };
            status.MovePiece(0, 3, move);
            var actual = status1.CloneAndMove(0, 3, move);
            Assert.AreNotSame(expected, actual, "Method cloned the board.");
            CompareBoards(expected, actual, "Movement was made on the cloned board.");
        }
    }
}
