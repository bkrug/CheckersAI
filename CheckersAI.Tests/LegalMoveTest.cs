using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CheckersAI.Models;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace CheckersAI.Tests
{
    [TestClass]
    public class LegalMoveTests
    {
        private Piece[,] _pieces = new Piece[8,8];

        [TestMethod]
        public void Moves_CanMoveForwards()
        {
            {
                var pieces = new Piece[8, 8];
                pieces[4, 3] = new Piece() { DownBoundTeam = true, IsKing = false };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = false }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece[8, 8];
                pieces[4, 3] = new Piece() { DownBoundTeam = false, IsKing = false };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = false }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece[8, 8];
                pieces[4, 3] = new Piece() { DownBoundTeam = true, IsKing = false };
                pieces[5, 4] = new Piece() { DownBoundTeam = true, IsKing = true };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece[8, 8];
                pieces[4, 3] = new Piece() { DownBoundTeam = false, IsKing = false };
                pieces[3, 4] = new Piece() { DownBoundTeam = false, IsKing = true };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false }}},
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece[8, 8];
                pieces[4, 3] = new Piece() { DownBoundTeam = true, IsKing = false };
                pieces[5, 4] = new Piece() { DownBoundTeam = true, IsKing = false };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece[8, 8];
                pieces[4, 3] = new Piece() { DownBoundTeam = false, IsKing = false };
                pieces[3, 4] = new Piece() { DownBoundTeam = false, IsKing = false };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false }}},
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece[8, 8];
                pieces[4, 3] = new Piece() { DownBoundTeam = true, IsKing = false };
                pieces[5, 4] = new Piece() { DownBoundTeam = false, IsKing = false };
                pieces[6, 5] = new Piece() { DownBoundTeam = false, IsKing = false };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece[8, 8];
                pieces[4, 3] = new Piece() { DownBoundTeam = true, IsKing = false };
                pieces[5, 2] = new Piece() { DownBoundTeam = true, IsKing = true };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = false }}},
                };
                CompareMoves(pieces, expectedMoves);
            }
        }

        private void CompareMoves(Piece[,] pieces, List<Move> expectedMoves)
        {
            CompareMoves(pieces, expectedMoves, 4, 3);
        }

        private void CompareMoves(Piece[,] pieces, List<Move> expectedMoves, int row, int column)
        {
            var legalMoveFinder = new LegalMoveFinder(pieces);
            var legalMoves = legalMoveFinder.GetLegalMoves(row, column);
            expectedMoves = SortMoves(expectedMoves);
            legalMoves = SortMoves(legalMoves);

            Assert.AreEqual(expectedMoves.Count, legalMoves.Count, "Correct number of results.");
            for (var i = 0; i < expectedMoves.Count; ++i)
            {
                Assert.AreEqual(expectedMoves[i].Steps.Count(), legalMoves[i].Steps.Count(), "Correct number of steps within move");
                for (var j = 0; j < expectedMoves[i].Steps.Count(); ++j)
                {
                    Assert.AreEqual(expectedMoves[i].Steps.ElementAt(j).Direction, legalMoves[i].Steps.ElementAt(j).Direction, "Correct direction");
                    Assert.AreEqual(expectedMoves[i].Steps.ElementAt(j).Jump, legalMoves[i].Steps.ElementAt(j).Jump, "Correct jump state.");
                }
            }
        }

        private List<Move> SortMoves(List<Move> moves) {
            return moves
                .OrderBy(m => m.Steps.Count())
                .ThenBy(m => m.Steps.First().Direction)
                .ThenBy(m => m.Steps.First().Jump)
                .ThenBy(m => m.Steps.Count() > 1 ? m.Steps.ElementAt(1).Direction : 0)
                .ThenBy(m => m.Steps.Count() > 1 ? m.Steps.ElementAt(1).Jump : true)
                .ThenBy(m => m.Steps.Count() > 2 ? m.Steps.ElementAt(2).Direction : 0)
                .ThenBy(m => m.Steps.Count() > 2 ? m.Steps.ElementAt(2).Jump : true)
                .ThenBy(m => m.Steps.Count() > 3 ? m.Steps.ElementAt(3).Direction : 0)
                .ThenBy(m => m.Steps.Count() > 3 ? m.Steps.ElementAt(3).Jump : true)
                .ThenBy(m => m.Steps.Count() > 4 ? m.Steps.ElementAt(4).Direction : 0)
                .ThenBy(m => m.Steps.Count() > 4 ? m.Steps.ElementAt(4).Jump : true)
                .ToList();
        }

        [TestMethod]
        public void Moves_KingCanMove()
        {
            {
                var pieces = new Piece[8, 8];
                pieces[4, 3] = new Piece() { DownBoundTeam = true, IsKing = true };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = false }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece[8, 8];
                pieces[4, 3] = new Piece() { DownBoundTeam = false, IsKing = true };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = false }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece[8, 8];
                pieces[4, 3] = new Piece() { DownBoundTeam = true, IsKing = true };
                pieces[5, 4] = new Piece() { DownBoundTeam = true, IsKing = true };
                pieces[3, 4] = new Piece() { DownBoundTeam = true, IsKing = true };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece[8, 8];
                pieces[4, 3] = new Piece() { DownBoundTeam = false, IsKing = true };
                pieces[5, 4] = new Piece() { DownBoundTeam = false, IsKing = true };
                pieces[3, 4] = new Piece() { DownBoundTeam = false, IsKing = true };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece[8, 8];
                pieces[4, 3] = new Piece() { DownBoundTeam = true, IsKing = true };
                pieces[5, 4] = new Piece() { DownBoundTeam = true, IsKing = true };
                pieces[5, 2] = new Piece() { DownBoundTeam = true, IsKing = true };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = false }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece[8, 8];
                pieces[4, 3] = new Piece() { DownBoundTeam = true, IsKing = true };
                pieces[5, 4] = new Piece() { DownBoundTeam = false, IsKing = true };
                pieces[6, 5] = new Piece() { DownBoundTeam = false, IsKing = true };
                pieces[5, 2] = new Piece() { DownBoundTeam = true, IsKing = true };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = false }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
        }

        [TestMethod]
        public void Moves_CanJump()
        {
            {
                var pieces = new Piece[8, 8];
                pieces[4, 3] = new Piece() { DownBoundTeam = true, IsKing = false };
                pieces[5, 4] = new Piece() { DownBoundTeam = false, IsKing = false };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece[8, 8];
                pieces[4, 3] = new Piece() { DownBoundTeam = true, IsKing = false };
                pieces[5, 4] = new Piece() { DownBoundTeam = false, IsKing = true };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece[8, 8];
                pieces[4, 3] = new Piece() { DownBoundTeam = true, IsKing = false };
                pieces[5, 4] = new Piece() { DownBoundTeam = false, IsKing = false };
                pieces[5, 2] = new Piece() { DownBoundTeam = true, IsKing = false };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece[8, 8];
                pieces[4, 3] = new Piece() { DownBoundTeam = true, IsKing = false };
                pieces[5, 2] = new Piece() { DownBoundTeam = false, IsKing = false };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = true}}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = false }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece[8, 8];
                pieces[2, 3] = new Piece() { DownBoundTeam = true, IsKing = false };
                pieces[3, 4] = new Piece() { DownBoundTeam = false, IsKing = false };
                pieces[5, 6] = new Piece() { DownBoundTeam = false, IsKing = true };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}}
                };
                CompareMoves(pieces, expectedMoves, 2, 3);
            }
            {
                var pieces = new Piece[8, 8];
                pieces[0, 1] = new Piece() { DownBoundTeam = true, IsKing = false };
                pieces[1, 2] = new Piece() { DownBoundTeam = false, IsKing = false };
                pieces[3, 4] = new Piece() { DownBoundTeam = false, IsKing = true };
                pieces[5, 6] = new Piece() { DownBoundTeam = false, IsKing = false };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}}
                };
                CompareMoves(pieces, expectedMoves, 0, 1);
            }
            {
                var pieces = new Piece[8, 8];
                pieces[0, 1] = new Piece() { DownBoundTeam = true, IsKing = false };
                pieces[1, 2] = new Piece() { DownBoundTeam = false, IsKing = false };
                pieces[3, 4] = new Piece() { DownBoundTeam = false, IsKing = true };
                pieces[5, 4] = new Piece() { DownBoundTeam = false, IsKing = false };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = true }}}
                };
                CompareMoves(pieces, expectedMoves, 0 , 1);
            }
            {
                var pieces = new Piece[8, 8];
                pieces[0, 1] = new Piece() { DownBoundTeam = true, IsKing = false };
                pieces[1, 2] = new Piece() { DownBoundTeam = false, IsKing = false };
                pieces[3, 2] = new Piece() { DownBoundTeam = false, IsKing = true };
                pieces[5, 2] = new Piece() { DownBoundTeam = false, IsKing = false };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = true }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}}
                };
                CompareMoves(pieces, expectedMoves, 0, 1);
            }
            {
                var pieces = new Piece[8, 8];
                pieces[4, 3] = new Piece() { DownBoundTeam = false, IsKing = false };
                pieces[3, 4] = new Piece() { DownBoundTeam = true, IsKing = false };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = true }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece[8, 8];
                pieces[4, 3] = new Piece() { DownBoundTeam = false, IsKing = false };
                pieces[3, 4] = new Piece() { DownBoundTeam = true, IsKing = false };
                pieces[1, 4] = new Piece() { DownBoundTeam = true, IsKing = true };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = true }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = true }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece[8, 8];
                pieces[2, 3] = new Piece() { DownBoundTeam = true, IsKing = false };
                pieces[3, 4] = new Piece() { DownBoundTeam = false, IsKing = false };
                pieces[5, 4] = new Piece() { DownBoundTeam = false, IsKing = true };
                pieces[5, 6] = new Piece() { DownBoundTeam = false, IsKing = true };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = true }}},
                };
                CompareMoves(pieces, expectedMoves, 2, 3);
            }
        }

        [TestMethod]
        public void Moves_KingCanJump()
        {
            {
                var pieces = new Piece[8, 8];
                pieces[4, 3] = new Piece() { DownBoundTeam = true, IsKing = true };
                pieces[5, 4] = new Piece() { DownBoundTeam = false, IsKing = false };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece[8, 8];
                pieces[4, 3] = new Piece() { DownBoundTeam = false, IsKing = true };
                pieces[5, 4] = new Piece() { DownBoundTeam = true, IsKing = false };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece[8, 8];
                pieces[4, 3] = new Piece() { DownBoundTeam = true, IsKing = true };
                pieces[5, 4] = new Piece() { DownBoundTeam = false, IsKing = false };
                pieces[3, 2] = new Piece() { DownBoundTeam = false, IsKing = false };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = true }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece[8, 8];
                pieces[4, 3] = new Piece() { DownBoundTeam = true, IsKing = true };
                pieces[5, 4] = new Piece() { DownBoundTeam = false, IsKing = false };
                pieces[5, 6] = new Piece() { DownBoundTeam = false, IsKing = true };
                pieces[3, 6] = new Piece() { DownBoundTeam = false, IsKing = false };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = true }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = true }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece[8, 8];
                pieces[2, 3] = new Piece() { DownBoundTeam = true, IsKing = true };
                pieces[3, 4] = new Piece() { DownBoundTeam = false, IsKing = false };
                pieces[3, 6] = new Piece() { DownBoundTeam = false, IsKing = true };
                pieces[1, 6] = new Piece() { DownBoundTeam = false, IsKing = false };
                pieces[1, 4] = new Piece() { DownBoundTeam = false, IsKing = false };
                pieces[3, 2] = new Piece() { DownBoundTeam = false, IsKing = false };
                pieces[5, 2] = new Piece() { DownBoundTeam = false, IsKing = false };
                pieces[5, 4] = new Piece() { DownBoundTeam = false, IsKing = false };
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = true }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = true }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = true }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = true }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = true }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = true }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}}
                };
                var legalMoveFinder = new LegalMoveFinder(pieces);
                var legalMoves = legalMoveFinder.GetLegalMoves(2, 3);
                expectedMoves = SortMoves(expectedMoves);
                legalMoves = SortMoves(legalMoves.Where(m => m.Steps.Count == 1 || m.Steps.Count == 7).ToList());

                Assert.AreEqual(expectedMoves.Count, legalMoves.Count, "Correct number of results.");
                for (var i = 0; i < expectedMoves.Count; ++i)
                {
                    Assert.AreEqual(expectedMoves[i].Steps.Count(), legalMoves[i].Steps.Count(), "Correct number of steps within move");
                    for (var j = 0; j < expectedMoves[i].Steps.Count(); ++j)
                    {
                        Assert.AreEqual(expectedMoves[i].Steps.ElementAt(j).Direction, legalMoves[i].Steps.ElementAt(j).Direction, "Correct direction");
                        Assert.AreEqual(expectedMoves[i].Steps.ElementAt(j).Jump, legalMoves[i].Steps.ElementAt(j).Jump, "Correct jump state.");
                    }
                }
            }
        }
    }
}
