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
        private int _lowPlanDepth = 6;

        [TestMethod]
        public void Planner_GetNextMove_OneTurn()
        {
            var pieces = new Piece?[8, 8];
            pieces[4, 3] = Piece.DOWN_TEAM;
            pieces[5, 4] = Piece.UP_TEAM;
            var planner = new MovePlanner(pieces, _lowPlanDepth);
            var expectedMoveForDown = new MovePlan()
            {
                StartRow = 4,
                StartColumn = 3,
                Move = new Move() { Steps = new List<MoveStep>() { 
                    new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true } } },
                Heuristic = MovePlanner.WinHeuristic
            };
            var expectedMoveForUp = new MovePlan()
            {
                StartRow = 5,
                StartColumn = 4,
                Move = new Move() { Steps = new List<MoveStep>() { 
                    new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = true } }
                },
                Heuristic = MovePlanner.WinHeuristic
            };
            var actualDownMove = planner.GetNextMove(true);
            CompareRecommendedMoves(expectedMoveForDown, actualDownMove, "Down should win in one move.");
            var actualUpMove = planner.GetNextMove(false);
            CompareRecommendedMoves(expectedMoveForUp, actualUpMove, "Up should win in one move.");
        }

        [TestMethod]
        public void Planner_GetNextMove_TwoTurn()
        {
            {
                var pieces = new Piece?[8, 8];
                pieces[2, 4] = Piece.DOWN_TEAM;
                pieces[5, 3] = Piece.UP_TEAM;
                var planner = new MovePlanner(pieces, _lowPlanDepth);
                var expectedMove = new MovePlan()
                {
                    StartRow = 2,
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
                pieces[2, 4] = Piece.DOWN_TEAM;
                pieces[3, 1] = Piece.DOWN_TEAM;
                pieces[5, 3] = Piece.UP_TEAM;
                var planner = new MovePlanner(pieces, _lowPlanDepth);
                var okMoves = new List<MovePlan>();
                okMoves.Add(new MovePlan()
                {
                    StartRow = 2,
                    StartColumn = 4,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false } }
                    },
                    Heuristic = MovePlanner.WinHeuristic
                });
                okMoves.Add(new MovePlan()
                {
                    StartRow = 2,
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
                pieces[2, 4] = Piece.DOWN_TEAM;
                pieces[2, 6] = Piece.DOWN_TEAM;
                pieces[3, 1] = Piece.DOWN_TEAM;
                pieces[5, 3] = Piece.UP_TEAM;
                var planner = new MovePlanner(pieces, _lowPlanDepth);
                var okMoves = new List<MovePlan>();
                okMoves.Add(new MovePlan()
                {
                    StartRow = 2,
                    StartColumn = 6,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false } }
                    },
                    Heuristic = MovePlanner.WinHeuristic
                });
                okMoves.Add(new MovePlan()
                {
                    StartRow = 2,
                    StartColumn = 4,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false } }
                    },
                    Heuristic = MovePlanner.WinHeuristic
                });
                okMoves.Add(new MovePlan()
                {
                    StartRow = 2,
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
                pieces[2, 0] = Piece.DOWN_TEAM;
                pieces[2, 4] = Piece.DOWN_TEAM;
                pieces[2, 6] = Piece.DOWN_TEAM;
                pieces[6, 4] = Piece.UP_TEAM;
                var planner = new MovePlanner(pieces, _lowPlanDepth);
                var okMoves = new List<MovePlan>();
                okMoves.Add(new MovePlan()
                {
                    StartRow = 2,
                    StartColumn = 0,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = false } }
                    },
                    Heuristic = MovePlanner.WinHeuristic
                });
                okMoves.Add(new MovePlan()
                {
                    StartRow = 2,
                    StartColumn = 4,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false } }
                    },
                    Heuristic = MovePlanner.WinHeuristic
                });
                okMoves.Add(new MovePlan()
                {
                    StartRow = 2,
                    StartColumn = 4,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = false } }
                    },
                    Heuristic = MovePlanner.WinHeuristic
                });
                okMoves.Add(new MovePlan()
                {
                    StartRow = 2,
                    StartColumn = 6,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false } }
                    },
                    Heuristic = MovePlanner.WinHeuristic
                });
                okMoves.Add(new MovePlan()
                {
                    StartRow = 2,
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
    }
}
