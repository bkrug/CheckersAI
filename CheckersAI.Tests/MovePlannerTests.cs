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
        [TestMethod]
        public void Planner_GetNextMove_OneTurn()
        {
            var pieces = new Piece?[8, 8];
            pieces[4, 3] = Piece.DOWN_TEAM;
            pieces[5, 4] = Piece.UP_TEAM;
            var planner = new MovePlanner(pieces);
            var expectedMoveForDown = new MovePlan()
            {
                StartRow = 4,
                StartColumn = 3,
                Move = new Move() { Steps = new List<MoveStep>() { 
                    new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true } } },
                Wins = 1,
                Loses = 0,
                Incomplete = 0
            };
            var expectedMoveForUp = new MovePlan()
            {
                StartRow = 5,
                StartColumn = 4,
                Move = new Move() { Steps = new List<MoveStep>() { 
                    new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = true } } },
                Wins = 1,
                Loses = 0,
                Incomplete = 0
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
                var planner = new MovePlanner(pieces);
                var expectedMove = new MovePlan()
                {
                    StartRow = 2,
                    StartColumn = 4,
                    Move = new Move()
                    {
                        Steps = new List<MoveStep>() { new MoveStep() { Direction  = MoveDirection.DOWN_LEFT, Jump = false } }
                    },
                    Wins = 2,
                    Loses = 0,
                    Incomplete = 0
                };
                var actualMove = planner.GetNextMove(true);
                CompareRecommendedMoves(expectedMove, actualMove, "One-to-one wind.");
            }
            //{
            //    var pieces = new Piece?[8, 8];
            //    pieces[2, 4] = Piece.DOWN_TEAM;
            //    pieces[3, 1] = Piece.DOWN_TEAM;
            //    pieces[5, 3] = Piece.UP_TEAM;
            //    var planner = new MovePlanner(pieces);
            //    var expectedMove = new RecommendedMove()
            //    {
            //        Row = 2,
            //        Column = 4,
            //        Move = new Move()
            //        {
            //            Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false } }
            //        }
            //    };
            //    var actualMove = planner.GetNextMove(true);
            //    CompareRecommendedMoves(expectedMove, actualMove, "One-to-one wind.");
            //}
        }

        private void CompareRecommendedMoves(MovePlan expected, MovePlan actual, string message)
        {
            if (expected.StartRow != actual.StartRow)
                Assert.Fail(message + " StartRow is wrong.");
            if (expected.StartColumn != actual.StartColumn)
                Assert.Fail(message + " EndRow is wrong.");
            if (expected.Wins != actual.Wins)
                Assert.Fail(message + " Wins is wrong.");
            if (expected.Loses != actual.Loses)
                Assert.Fail(message + " Loses is wrong.");
            if (expected.Incomplete != actual.Incomplete)
                Assert.Fail(message + " Incomplete is wrong.");
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
    }
}
