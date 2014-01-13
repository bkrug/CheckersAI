using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CheckersAI.Models;
using System.Collections.Generic;

namespace CheckersAI.Tests
{
    [TestClass]
    public class GameBoardTests
    {
        [TestMethod]
        public void Board_Constructor()
        {
            try
            {
                var board = new GameBoard(new Piece?[7, 8]);
                Assert.Fail("Exception should be thrown when board is not wide enough.");
            }
            catch (InvalidBoardException ex)
            {
                Assert.IsTrue(true, "Exception thrown when board is not wide enough.");
            }
            catch (Exception ex)
            {
                Assert.Fail("Wrong exception thrown when board is not wide enough.");
            }
            try
            {
                var board = new GameBoard(new Piece?[8, 7]);
                Assert.Fail("Exception should be thrown when board is not tall enough.");
            }
            catch (InvalidBoardException ex)
            {
                Assert.IsTrue(true, "Exception thrown when board is not tall enough.");
            }
            catch (Exception ex)
            {
                Assert.Fail("Wrong exception thrown when board is not tall enough.");
            }
            try
            {
                var board = new GameBoard(new Piece?[9, 8]);
                Assert.Fail("Exception should be thrown when board is too wide enough.");
            }
            catch (InvalidBoardException ex)
            {
                Assert.IsTrue(true, "Exception thrown when board is too wide enough.");
            }
            catch (Exception ex)
            {
                Assert.Fail("Wrong exception thrown when board is too wide enough.");
            }
            try
            {
                var board = new GameBoard(new Piece?[8, 9]);
                Assert.Fail("Exception should be thrown when board is too tall enough.");
            }
            catch (InvalidBoardException ex)
            {
                Assert.IsTrue(true, "Exception thrown when board is too tall enough.");
            }
            catch (Exception ex)
            {
                Assert.Fail("Wrong exception thrown when board is too tall enough.");
            }
            {
                var board = new GameBoard(new Piece?[8, 8]);
                Assert.IsTrue(true, "Exception should not be thrown when board is 8x8.");
            }
        }

        [TestMethod]
        public void Board_Pieces()
        {
            var pieces = new Piece?[8, 8];
            pieces[4, 2] = Piece.UP_TEAM_KING;
            var board = new GameBoard(pieces);
            Assert.AreEqual(pieces[3, 1], board[3, 1], "The constructor did not change the parameter");
            Assert.AreEqual(pieces[4, 2], board[4, 2], "The constructor did not change the parameter");
        }

        [TestMethod]
        public void Board_WinnerByElimination()
        {
            {
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.DOWN_TEAM;
                pieces[5, 6] = Piece.DOWN_TEAM;
                pieces[2, 1] = Piece.DOWN_TEAM;
                var status1 = new GameBoard(pieces);
                Assert.AreEqual(true, status1.WinnerByElimination, "DownBoundTeam is calculated as winner.");
                status1[7, 2] = Piece.UP_TEAM;
                Assert.AreEqual(null, status1.WinnerByElimination, "No team is calculated as winner.");
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.UP_TEAM;
                pieces[5, 6] = Piece.UP_TEAM;
                pieces[2, 1] = Piece.UP_TEAM;
                var status1 = new GameBoard(pieces);
                Assert.AreEqual(false, status1.WinnerByElimination, "DownBoundTeam is calculated as loser.");
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[2, 4] = Piece.DOWN_TEAM;
                pieces[3, 1] = Piece.DOWN_TEAM;
                pieces[5, 7] = Piece.UP_TEAM;
                var status1 = new GameBoard(pieces);
                Assert.AreEqual(null, status1.WinnerByElimination, "No team is winner.");
            }
        }

        [TestMethod]
        public void Board_WinnerByDeadlock()
        {
            {
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.DOWN_TEAM;
                pieces[5, 2] = Piece.UP_TEAM;
                pieces[5, 4] = Piece.UP_TEAM;
                pieces[6, 1] = Piece.UP_TEAM;
                pieces[6, 5] = Piece.UP_TEAM;
                var status1 = new GameBoard(pieces);
                Assert.AreEqual(false, status1.WinnerByGridlock, "UpBoundTeam is calculated as winner.");
                status1[1, 2] = Piece.DOWN_TEAM;
                Assert.AreEqual(null, status1.WinnerByGridlock, "No team is calculated as winner.");
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[6, 3] = Piece.UP_TEAM;
                pieces[5, 2] = Piece.DOWN_TEAM;
                pieces[5, 4] = Piece.DOWN_TEAM;
                pieces[4, 1] = Piece.DOWN_TEAM;
                pieces[4, 5] = Piece.DOWN_TEAM;
                var status1 = new GameBoard(pieces);
                Assert.AreEqual(true, status1.WinnerByGridlock, "UpBoundTeam is calculated as winner.");
            }
            {
                var pieces = new Piece?[8, 8];
                var status1 = new GameBoard(pieces);
                pieces[2, 4] = Piece.DOWN_TEAM;
                pieces[3, 1] = Piece.DOWN_TEAM;
                pieces[5, 7] = Piece.UP_TEAM;
                Assert.AreEqual(null, status1.WinnerByGridlock, "No team is winner.");
            }
        }

        [TestMethod]
        public void Board_MovePiece()
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
                var status = new GameBoard(pieces);

                var expectedBoard = new GameBoard((Piece?[,])pieces.Clone());
                expectedBoard[5, 2] = expectedBoard[4, 3];
                expectedBoard[4, 3] = null;
                status.MovePiece(4, 3, new Move() { Steps = new List<MoveStep> { new MoveStep() { Direction = MoveDirection.DOWN_LEFT } } });
                CompareBoards(expectedBoard, status, "Piece moved down-left.");

                expectedBoard[5, 6] = expectedBoard[4, 5];
                expectedBoard[4, 5] = null;
                status.MovePiece(4, 5, new Move() { Steps = new List<MoveStep> { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT } } });
                CompareBoards(expectedBoard, status, "Piece moved down-right.");

                expectedBoard[2, 1] = expectedBoard[3, 2];
                expectedBoard[3, 2] = null;
                status.MovePiece(3, 2, new Move() { Steps = new List<MoveStep> { new MoveStep() { Direction = MoveDirection.UP_LEFT } } });
                CompareBoards(expectedBoard, status, "Piece moved up-left");

                expectedBoard[2, 5] = expectedBoard[3, 4];
                expectedBoard[3, 4] = null;
                status.MovePiece(3, 4, new Move() { Steps = new List<MoveStep> { new MoveStep() { Direction = MoveDirection.UP_RIGHT } } });
                CompareBoards(expectedBoard, status, "Piece moved up-right.");

                expectedBoard[7, 4] = PieceUtil.King(expectedBoard[6, 3].Value);
                expectedBoard[6, 3] = null;
                status.MovePiece(6, 3, new Move() { Steps = new List<MoveStep> { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT } } });
                CompareBoards(expectedBoard, status, "Piece moved down-right and kinged.");

                expectedBoard[0, 1] = PieceUtil.King(expectedBoard[1, 2].Value);
                expectedBoard[1, 2] = null;
                status.MovePiece(1, 2, new Move() { Steps = new List<MoveStep> { new MoveStep() { Direction = MoveDirection.UP_LEFT } } });
                CompareBoards(expectedBoard, status, "Piece moved up-left and kinged.");
            }
        }

        [TestMethod]
        public void Board_MovePiece2()
        {
            {
                var pieces = new Piece?[8, 8];
                pieces[0, 3] = Piece.DOWN_TEAM;
                pieces[1, 4] = Piece.UP_TEAM;
                pieces[3, 4] = Piece.UP_TEAM;
                pieces[7, 0] = Piece.UP_TEAM;
                pieces[6, 1] = Piece.DOWN_TEAM;
                pieces[7, 2] = Piece.UP_TEAM;
                var status = new GameBoard(pieces);

                var expectedBoard = new GameBoard((Piece?[,])pieces.Clone());
                expectedBoard[5, 2] = expectedBoard[7, 0];
                expectedBoard[7, 0] = null;
                expectedBoard[6, 1] = null;
                status.MovePiece(7, 0, new Move()
                {
                    Steps = new List<MoveStep> { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = true } }
                });
                CompareBoards(expectedBoard, status, "Captured one piece.");
                Assert.AreEqual(1, status.DownPieces, "One down piece was removed.");
                Assert.AreEqual(4, status.UpPieces, "Four up pieces are still there.");

                expectedBoard[4, 3] = expectedBoard[0, 3];
                expectedBoard[0, 3] = null;
                expectedBoard[1, 4] = null;
                expectedBoard[3, 4] = null;
                status.MovePiece(0, 3, new Move()
                {
                    Steps = new List<MoveStep> { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true },
                                                    new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = true } }
                });
                CompareBoards(expectedBoard, status, "Captured two pieces.");
                Assert.AreEqual(1, status.DownPieces, "Two down pieces are still there.");
                Assert.AreEqual(2, status.UpPieces, "Two up pieces were removed.");
            }
        }

        private void CompareBoards(GameBoard expected, GameBoard actual, string message)
        {
            for (var r = 0; r < 8; ++r)
                for (var c = 0; c < 8; ++c) 
                    if (expected[r, c] != actual[r, c])
                        Assert.Fail(message);
            Assert.IsTrue(true, message);
        }

        [TestMethod]
        public void Board_CloneBoardAndMove() {
            var expectedPieces = new Piece?[8, 8];
            expectedPieces[0, 3] = Piece.DOWN_TEAM;
            expectedPieces[1, 4] = Piece.UP_TEAM;
            expectedPieces[3, 4] = Piece.UP_TEAM;
            expectedPieces[7, 0] = Piece.UP_TEAM;
            expectedPieces[6, 1] = Piece.DOWN_TEAM;
            expectedPieces[7, 2] = Piece.UP_TEAM;
            var expectedBord = new GameBoard(expectedPieces);
            var clonedPieces = (Piece?[,])expectedPieces.Clone();
            var workerBoard = new GameBoard(clonedPieces);
            var move = new Move()
            {
                Steps = new List<MoveStep> { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true },
                                             new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = true } }
            };
            expectedBord.MovePiece(0, 3, move);
            var actualBoard = workerBoard.CloneAndMove(0, 3, move);
            Assert.AreNotSame(workerBoard, actualBoard, "Method cloned the board.");
            CompareBoards(expectedBord, actualBoard, "Movement was made on the cloned board.");
        }

        [TestMethod]
        public void Position_Legal()
        {
            var i = 0;
            var board = new GameBoard();
            var p1 = board[0, 1];
            Assert.IsTrue(true, "Position " + (++i) + " is valid.");
            var p2 = board[0, 7];
            Assert.IsTrue(true, "Position " + (++i) + " is valid.");
            var p3 = board[0, 3];
            Assert.IsTrue(true, "Position " + (++i) + " is valid.");
            var p4 = board[1, 2];
            Assert.IsTrue(true, "Position " + (++i) + " is valid.");
            var p5 = board[1, 6];
            Assert.IsTrue(true, "Position " + (++i) + " is valid.");
            var p6 = board[2, 5];
            Assert.IsTrue(true, "Position " + (++i) + " is valid.");
            var p7 = board[2, 7];
            Assert.IsTrue(true, "Position " + (++i) + " is valid.");
            var p8 = board[3, 4];
            Assert.IsTrue(true, "Position " + (++i) + " is valid.");
            var p9 = board[6, 3];
            Assert.IsTrue(true, "Position " + (++i) + " is valid.");
            var p10 = board[6, 7];
            Assert.IsTrue(true, "Position " + (++i) + " is valid.");
            var p11 = board[7, 2];
            Assert.IsTrue(true, "Position " + (++i) + " is valid.");
        }

        [TestMethod]
        public void Position_NotLegal() 
        {
            var offboard = new[] { new[] { 0, -4 }, new[] { -9, 3 }, new[] { 5, 9 }, new[] { 8, 4 }, new[] { -2, -4 }, new[] { -1, 9 }, new[] { 8, -4 } };
            var onboard = new[] { new[] { 0, 4 }, new[] { 3, 5 }, new[] { 7, 5 }, new[] { 6, 2 } };

            var board = new GameBoard();
            for (var i = 0; i < offboard.Length; ++i) 
            {
                try
                {
                    board[offboard[i][0], offboard[i][1]] = null;
                    Assert.Fail("Board Position " + offboard[i][0] + ", " + offboard[i][1] + " should fail since it is off the board");
                }
                catch (OffBoardException ex)
                {
                    Assert.IsTrue(true, "Board Position " + offboard[i][0] + ", " + offboard[i][1] + " fails since it is off the board");
                }
                catch (ApplicationException ex)
                {
                    Assert.Fail("When creating Board Position " + offboard[i][0] + ", " + offboard[i][1] + " wrong exception occurred.");
                }
            }
            for (var i = 0; i < onboard.Length; ++i)
            {
                try
                {
                    board[onboard[i][0], onboard[i][1]] = null;
                    Assert.Fail("Board Position " + onboard[i][0] + ", " + onboard[i][1] + " should fail since it is invalid.");
                }
                catch (InvalidPositionException ex)
                {
                    Assert.IsTrue(true, "Board Position " + offboard[i][0] + ", " + offboard[i][1] + " fails since it is invalid.");
                }
                catch (ApplicationException ex)
                {
                    Assert.Fail("When creating Board Position " + offboard[i][0] + ", " + offboard[i][1] + " wrong exception occurred.");
                }
            }
        }

    }
}
