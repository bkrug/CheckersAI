using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CheckersAI.Models;
using System.Collections.Generic;

namespace CheckersAI.Tests
{
    [TestClass]
    public class MovePlannerTests
    {
        private int _mediumPlanDepth = 6;

        [TestMethod]
        public void Planner_GetNextMove_OneTurn()
        {
            {
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.DOWN_TEAM;
                pieces[5, 4] = Piece.UP_TEAM;
                var board = new GameBoard(pieces);
                var planner = new MovePlanner(board, _mediumPlanDepth);
                var expectedMoveForDown = new MovePlan()
                {
                    StartRow = 4,
                    StartColumn = 3,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { 
                    new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true } }
                    },
                    Heuristic = MovePlanner.WinHeuristic
                };
                var expectedMoveForUp = new MovePlan()
                {
                    StartRow = 5,
                    StartColumn = 4,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { 
                    new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = true } }
                    },
                    Heuristic = -MovePlanner.WinHeuristic
                };
                var actualDownMove = planner.GetNextMove(true);
                CompareRecommendedMoves(expectedMoveForDown, actualDownMove, "Down should win in one move.");
                var actualUpMove = planner.GetNextMove(false);
                CompareRecommendedMoves(expectedMoveForUp, actualUpMove, "Up should win in one move.");
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[0, 3] = Piece.DOWN_TEAM;
                pieces[7, 4] = Piece.UP_TEAM;
                var board = new GameBoard(pieces);
                var planner = new MovePlanner(board, 2);
                var actualMove = planner.GetNextMove(true);
                Assert.AreEqual(0, actualMove.Heuristic, "No winner. Not enough time.");
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[0, 3] = Piece.DOWN_TEAM;
                pieces[1, 2] = Piece.UP_TEAM;
                pieces[7, 4] = Piece.UP_TEAM;
                var board = new GameBoard(pieces);
                var planner = new MovePlanner(board, 2);
                var expectedMove = new MovePlan()
                {
                    StartRow = 0,
                    StartColumn = 3,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = true } }
                    },
                    Heuristic = 0
                };
                var actualMove = planner.GetNextMove(true);
                CompareRecommendedMoves(expectedMove, actualMove, "No winner. But captured one piece.");
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[0, 3] = Piece.DOWN_TEAM;
                pieces[0, 5] = Piece.DOWN_TEAM; 
                pieces[1, 2] = Piece.UP_TEAM;
                pieces[7, 4] = Piece.UP_TEAM;
                var board = new GameBoard(pieces);
                var planner = new MovePlanner(board, 2);
                var expectedMove = new MovePlan()
                {
                    StartRow = 0,
                    StartColumn = 3,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = true } }
                    },
                    Heuristic = 1
                };
                var actualMove = planner.GetNextMove(true);
                CompareRecommendedMoves(expectedMove, actualMove, "No winner. But captured one piece making a gain.");
            }
        }

        [TestMethod]
        public void Planner_WinByGridlock()
        {
            {
                var pieces = new Piece?[8, 8];
                pieces[5, 0] = Piece.DOWN_TEAM;
                pieces[5, 2] = Piece.DOWN_TEAM;
                pieces[7, 0] = Piece.UP_TEAM;
                pieces[7, 2] = Piece.DOWN_TEAM_KING;
                var board = new GameBoard(pieces);
                var planner = new MovePlanner(board, 6);
                var okMoves = new List<MovePlan>();
                okMoves.Add(new MovePlan()
                {
                    StartRow = 7,
                    StartColumn = 2,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false } }
                    },
                    Heuristic = MovePlanner.WinHeuristic
                });
                okMoves.Add(new MovePlan()
                {
                    StartRow = 5,
                    StartColumn = 0,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = false } }
                    },
                    Heuristic = MovePlanner.WinHeuristic
                });
                var actualMove = planner.GetNextMove(true);
                var matchFound = false;
                foreach (var move in okMoves)
                {
                    if (AreMatches(move, actualMove))
                        matchFound = true;
                }
                Assert.IsTrue(matchFound, "Down team won by gridlock in one move.");
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[1, 6] = Piece.DOWN_TEAM;
                pieces[2, 5] = Piece.DOWN_TEAM;
                pieces[3, 4] = Piece.DOWN_TEAM;
                pieces[4, 5] = Piece.DOWN_TEAM_KING;
                pieces[5, 6] = Piece.UP_TEAM;
                pieces[6, 7] = Piece.DOWN_TEAM;
                var board = new GameBoard(pieces);
                var planner = new MovePlanner(board, 6);
                var expectedMove = new MovePlan()
                {
                    StartRow = 1,
                    StartColumn = 6,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = false } }
                    },
                    Heuristic = MovePlanner.WinHeuristic
                };
                var actualMove = planner.GetNextMove(true);
                CompareRecommendedMoves(expectedMove, actualMove, "Down wins by gridlock in two turns.");
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[2, 5] = Piece.DOWN_TEAM;
                pieces[2, 7] = Piece.DOWN_TEAM;
                pieces[3, 4] = Piece.DOWN_TEAM;
                pieces[4, 5] = Piece.DOWN_TEAM_KING;
                pieces[4, 7] = Piece.UP_TEAM;
                pieces[6, 7] = Piece.DOWN_TEAM;
                var board = new GameBoard(pieces);
                var planner = new MovePlanner(board, 6);
                var okMoves = new List<MovePlan>();
                okMoves.Add(new MovePlan()
                {
                    StartRow = 4,
                    StartColumn = 5,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = false } }
                    },
                    Heuristic = MovePlanner.WinHeuristic
                });
                okMoves.Add(new MovePlan()
                {
                    StartRow = 2,
                    StartColumn = 7,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false } }
                    },
                    Heuristic = MovePlanner.WinHeuristic

                });
                var actualMove = planner.GetNextMove(true);
                var matchFound = false;
                foreach (var move in okMoves)
                {
                    if (AreMatches(move, actualMove))
                        matchFound = true;
                }
                Assert.IsTrue(matchFound, "Down team won by gridlock in one move.");
            }
        }

        [TestMethod]
        public void Planner_GetNextMove_TwoTurn()
        {
            {
                var pieces = new Piece?[8, 8];
                pieces[3, 4] = Piece.DOWN_TEAM;
                pieces[6, 3] = Piece.UP_TEAM;
                var board = new GameBoard(pieces);
                var planner = new MovePlanner(board, _mediumPlanDepth);
                var expectedMove = new MovePlan()
                {
                    StartRow = 3,
                    StartColumn = 4,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction  = MoveDirection.DOWN_LEFT, Jump = false } }
                    },
                    Heuristic = MovePlanner.WinHeuristic
                };
                var actualMove = planner.GetNextMove(true);
                CompareRecommendedMoves(expectedMove, actualMove, "One-to-one win.");
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[3, 4] = Piece.DOWN_TEAM;
                pieces[4, 1] = Piece.DOWN_TEAM;
                pieces[6, 3] = Piece.UP_TEAM;
                var board = new GameBoard(pieces);
                var planner = new MovePlanner(board, _mediumPlanDepth);
                var okMoves = new List<MovePlan>();
                okMoves.Add(new MovePlan()
                {
                    StartRow = 3,
                    StartColumn = 4,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false } }
                    },
                    Heuristic = MovePlanner.WinHeuristic
                });
                okMoves.Add(new MovePlan()
                {
                    StartRow = 3,
                    StartColumn = 4,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = false } }
                    },
                    Heuristic = MovePlanner.WinHeuristic
                });
                var actualMove = planner.GetNextMove(true);
                var matchFound = false;
                foreach(var move in okMoves) {
                    if (AreMatches(move, actualMove))
                        matchFound = true;
                }
                Assert.IsTrue(matchFound, "Two-to-one win.");
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[3, 4] = Piece.DOWN_TEAM;
                pieces[3, 6] = Piece.DOWN_TEAM;
                pieces[4, 1] = Piece.DOWN_TEAM;
                pieces[6, 3] = Piece.UP_TEAM;
                var board = new GameBoard(pieces);
                var planner = new MovePlanner(board, _mediumPlanDepth);
                var okMoves = new List<MovePlan>();
                okMoves.Add(new MovePlan()
                {
                    StartRow = 3,
                    StartColumn = 6,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false } }
                    },
                    Heuristic = MovePlanner.WinHeuristic
                });
                okMoves.Add(new MovePlan()
                {
                    StartRow = 3,
                    StartColumn = 4,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false } }
                    },
                    Heuristic = MovePlanner.WinHeuristic
                });
                okMoves.Add(new MovePlan()
                {
                    StartRow = 3,
                    StartColumn = 4,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = false } }
                    },
                    Heuristic = MovePlanner.WinHeuristic
                });
                var actualMove = planner.GetNextMove(true);
                var matchFound = false;
                foreach (var move in okMoves)
                {
                    if (AreMatches(move, actualMove))
                        matchFound = true;
                }
                Assert.IsTrue(matchFound, "Three-to-one win.");
            }
        }

        [TestMethod]
        public void Planner_GetNextMove_ThreeTurns()
        {
            {
                var pieces = new Piece?[8, 8];
                pieces[1, 0] = Piece.DOWN_TEAM;
                pieces[1, 4] = Piece.DOWN_TEAM;
                pieces[1, 6] = Piece.DOWN_TEAM;
                pieces[5, 4] = Piece.UP_TEAM;
                var board = new GameBoard(pieces);
                var planner = new MovePlanner(board, _mediumPlanDepth);
                var okMoves = new List<MovePlan>();
                okMoves.Add(new MovePlan()
                {
                    StartRow = 1,
                    StartColumn = 0,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = false } }
                    },
                    Heuristic = MovePlanner.WinHeuristic
                });
                okMoves.Add(new MovePlan()
                {
                    StartRow = 1,
                    StartColumn = 4,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false } }
                    },
                    Heuristic = MovePlanner.WinHeuristic
                });
                okMoves.Add(new MovePlan()
                {
                    StartRow = 1,
                    StartColumn = 4,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = false } }
                    },
                    Heuristic = MovePlanner.WinHeuristic
                });
                okMoves.Add(new MovePlan()
                {
                    StartRow = 1,
                    StartColumn = 6,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false } }
                    },
                    Heuristic = MovePlanner.WinHeuristic
                });
                okMoves.Add(new MovePlan()
                {
                    StartRow = 1,
                    StartColumn = 6,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = false } }
                    },
                    Heuristic = MovePlanner.WinHeuristic
                });
                var actualMove = planner.GetNextMove(true);
                var matchFound = false;
                foreach (var move in okMoves)
                {
                    if (AreMatches(move, actualMove))
                        matchFound = true;
                }
                Assert.IsTrue(matchFound, "Three-to-one win.");
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[0, 3] = Piece.DOWN_TEAM;
                pieces[0, 5] = Piece.DOWN_TEAM;
                pieces[3, 2] = Piece.UP_TEAM;
                pieces[6, 5] = Piece.UP_TEAM;
                var board = new GameBoard(pieces);
                var planner = new MovePlanner(board, 4);
                var expectedMove = new MovePlan()
                {
                    StartRow = 0,
                    StartColumn = 3,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false } }
                    },
                    Heuristic = 0
                };
                var actualMove = planner.GetNextMove(true);
                CompareRecommendedMoves(expectedMove, actualMove, " Down should be able to block but not capture one piece.");
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[0, 3] = Piece.DOWN_TEAM;
                pieces[0, 5] = Piece.DOWN_TEAM;
                pieces[3, 2] = Piece.UP_TEAM;
                pieces[6, 5] = Piece.UP_TEAM;
                var board = new GameBoard(pieces);
                var planner = new MovePlanner(board, _mediumPlanDepth);
                var expectedMove = new MovePlan()
                {
                    StartRow = 0,
                    StartColumn = 3,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false } }
                    },
                    Heuristic = 1
                };
                var actualMove = planner.GetNextMove(true);
                CompareRecommendedMoves(expectedMove, actualMove, " Down should be able to block but not capture one piece.");
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[0, 3] = Piece.DOWN_TEAM;
                pieces[0, 5] = Piece.DOWN_TEAM;
                pieces[1, 2] = Piece.UP_TEAM;
                pieces[6, 5] = Piece.UP_TEAM;
                var board = new GameBoard(pieces);
                var planner = new MovePlanner(board, 4);
                var expectedMove = new MovePlan()
                {
                    StartRow = 0,
                    StartColumn = 3,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = true } }
                    },
                    Heuristic = 1
                };
                var actualMove = planner.GetNextMove(true);
                CompareRecommendedMoves(expectedMove, actualMove, " Down should be able to capture one piece and one is left behind.");
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[0, 3] = Piece.DOWN_TEAM;
                pieces[0, 5] = Piece.DOWN_TEAM;
                pieces[1, 2] = Piece.UP_TEAM;
                pieces[6, 5] = Piece.UP_TEAM;
                var board = new GameBoard(pieces);
                var planner = new MovePlanner(board, _mediumPlanDepth);
                var expectedMove = new MovePlan()
                {
                    StartRow = 0,
                    StartColumn = 3,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = true } }
                    },
                    Heuristic = MovePlanner.WinHeuristic
                };
                var actualMove = planner.GetNextMove(true);
                CompareRecommendedMoves(expectedMove, actualMove, " Down should be able to capture both pieces.");
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[1, 2] = Piece.DOWN_TEAM;
                pieces[6, 5] = Piece.DOWN_TEAM;
                pieces[7, 2] = Piece.UP_TEAM;
                pieces[7, 4] = Piece.UP_TEAM;
                var board = new GameBoard(pieces);
                var planner = new MovePlanner(board, 4);
                var expectedMove = new MovePlan()
                {
                    StartRow = 7,
                    StartColumn = 4,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = true } }
                    },
                    Heuristic = -1
                };
                var actualMove = planner.GetNextMove(false);
                CompareRecommendedMoves(expectedMove, actualMove, " Up should be able to capture one piece and one is left behind.");
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[1, 2] = Piece.DOWN_TEAM;
                pieces[6, 5] = Piece.DOWN_TEAM;
                pieces[7, 2] = Piece.UP_TEAM;
                pieces[7, 4] = Piece.UP_TEAM;
                var board = new GameBoard(pieces);
                var planner = new MovePlanner(board, _mediumPlanDepth);
                var expectedMove = new MovePlan()
                {
                    StartRow = 7,
                    StartColumn = 4,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = true } }
                    },
                    Heuristic = -MovePlanner.WinHeuristic
                };
                var actualMove = planner.GetNextMove(false);
                CompareRecommendedMoves(expectedMove, actualMove, " Up should be able to capture both pieces.");
            }
        }

        private void CompareRecommendedMoves(MovePlan expected, MovePlan actual, string message)
        {
            Assert.AreEqual(expected.StartRow, actual.StartRow, message + " StartRow is wrong.");
            Assert.AreEqual(expected.StartColumn, actual.StartColumn, message + " EndRow is wrong.");
            Assert.AreEqual(expected.Heuristic, actual.Heuristic, message + " Heuristic is wrong.");
            CompareMoves(expected.Move, actual.Move, message);
            Assert.IsTrue(true, message + " Move is wrong.");
        }

        private void CompareMoves(Move expected, Move actual, string message)
        {
            var isMatch = true;
            if (expected.Steps.Count == actual.Steps.Count)
            {
                for (var i = 0; i < expected.Steps.Count && isMatch; ++i)
                {
                    isMatch =
                        expected.Steps[i].Direction == actual.Steps[i].Direction
                        && expected.Steps[i].Jump == actual.Steps[i].Jump;
                }
            }
            else
                isMatch = false;
            if (isMatch)
                Assert.IsTrue(true, message);
            else
                Assert.Fail("Expected " + expected.ToString() + " but actual " + actual.ToString() + " " + message);
        }

        private bool AreMatches(MovePlan expected, MovePlan actual)
        {
            if (expected.StartRow != actual.StartRow)
                return false;
            if (expected.StartColumn != actual.StartColumn)
                return false;
            if (expected.Heuristic != actual.Heuristic)
                return false;
            return AreMatches(expected.Move, actual.Move);
        }

        private bool AreMatches(Move expected, Move actual)
        {
            var isMatch = true;
            if (expected.Steps.Count == actual.Steps.Count)
            {
                for (var i = 0; i < expected.Steps.Count && isMatch; ++i)
                {
                    isMatch =
                        expected.Steps[i].Direction == actual.Steps[i].Direction
                        && expected.Steps[i].Jump == actual.Steps[i].Jump;
                }
            }
            else
                isMatch = false;
            if (isMatch)
                return true;
            else
                return false;
        }

        [TestMethod]
        public void Planner_GetNextMove_GameStart()
        {
            {
                var pieces = new Piece?[8, 8];
                pieces[0, 1] = Piece.DOWN_TEAM;
                pieces[0, 3] = Piece.DOWN_TEAM;
                pieces[0, 5] = Piece.DOWN_TEAM;
                pieces[0, 7] = Piece.DOWN_TEAM;
                pieces[1, 0] = Piece.DOWN_TEAM;
                pieces[1, 2] = Piece.DOWN_TEAM;
                pieces[1, 4] = Piece.DOWN_TEAM;
                pieces[1, 6] = Piece.DOWN_TEAM;
                pieces[2, 1] = Piece.DOWN_TEAM;
                pieces[2, 3] = Piece.DOWN_TEAM;
                pieces[2, 5] = Piece.DOWN_TEAM;
                pieces[2, 7] = Piece.DOWN_TEAM;
                pieces[5, 0] = Piece.UP_TEAM;
                pieces[5, 2] = Piece.UP_TEAM;
                pieces[5, 4] = Piece.UP_TEAM;
                pieces[5, 6] = Piece.UP_TEAM;
                pieces[6, 1] = Piece.UP_TEAM;
                pieces[6, 3] = Piece.UP_TEAM;
                pieces[6, 5] = Piece.UP_TEAM;
                pieces[6, 7] = Piece.UP_TEAM;
                pieces[7, 0] = Piece.UP_TEAM;
                pieces[7, 2] = Piece.UP_TEAM;
                pieces[7, 4] = Piece.UP_TEAM;
                pieces[7, 6] = Piece.UP_TEAM;
                var board = new GameBoard(pieces);
                var planner = new MovePlanner(board, 3);
                var nextMove = planner.GetNextMove(true);
            }
        }
    }
}
