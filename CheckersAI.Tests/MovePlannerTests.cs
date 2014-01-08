﻿using System;
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
        public void Planner_GetNextMove()
        {
            var pieces = new Piece?[8, 8];
            pieces[4, 3] = Piece.DOWN_TEAM;
            pieces[5, 4] = Piece.UP_TEAM;
            var planner = new MovePlanner(pieces);
            var expectedMoveForDown = new RecommendedMove()
            {
                Row = 4,
                Column = 3,
                Move = new Move() { Steps = new List<MoveStep>() { 
                    new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true } } }
            };
            var expectedMoveForUp = new RecommendedMove()
            {
                Row = 5,
                Column = 4,
                Move = new Move() { Steps = new List<MoveStep>() { 
                    new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = true } } }
            };
            var actualDownMove = planner.GetNextMove(true);
            CompareRecommendedMoves(expectedMoveForDown, actualDownMove, "Down should win in one move.");
            var actualUpMove = planner.GetNextMove(false);
            CompareRecommendedMoves(expectedMoveForUp, actualUpMove, "Up should win in one move.");
        }

        private void CompareRecommendedMoves(RecommendedMove expected, RecommendedMove actual, string message)
        {
            if (expected.Row != actual.Row && expected.Column != actual.Column)
                Assert.Fail(message);
            CompareMoves(expected.Move, actual.Move, message);
            Assert.IsTrue(true, message);
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
