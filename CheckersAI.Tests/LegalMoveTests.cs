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
        private Piece?[,] _pieces = new Piece?[8,8];

        [TestMethod]
        public void Moves_CanMoveForwards()
        {
            {
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.DOWN_TEAM;
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = false }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.UP_TEAM;
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = false }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.DOWN_TEAM;
                pieces[5, 4] = Piece.DOWN_TEAM_KING;
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.UP_TEAM;
                pieces[3, 4] = Piece.UP_TEAM_KING;
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false }}},
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.DOWN_TEAM;
                pieces[5, 4] = Piece.DOWN_TEAM;
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.UP_TEAM;
                pieces[3, 4] = Piece.UP_TEAM;
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false }}},
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.DOWN_TEAM;
                pieces[5, 4] = Piece.UP_TEAM;
                pieces[6, 5] = Piece.UP_TEAM;
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.DOWN_TEAM;
                pieces[5, 2] = Piece.DOWN_TEAM_KING;
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = false }}},
                };
                CompareMoves(pieces, expectedMoves);
            }
        }

        private void CompareMoves(Piece?[,] pieces, List<Move> expectedMoves)
        {
            CompareMoves(pieces, expectedMoves, 4, 3);
        }

        private void CompareMoves(Piece?[,] pieces, List<Move> expectedMoves, int row, int column)
        {
            var legalMoveFinder = new LegalMoveFinder(new GameBoard(pieces));
            var legalMoves = legalMoveFinder.GetLegalMoves(row, column);
        }

        private void CompareMoves(List<Move> expectedMoves, List<Move> actualMoves, int row, int column)
        {
            expectedMoves = SortMoves(expectedMoves);
            actualMoves = SortMoves(actualMoves);

            Assert.AreEqual(expectedMoves.Count, actualMoves.Count, "Correct number of results.");
            for (var i = 0; i < expectedMoves.Count; ++i)
            {
                Assert.AreEqual(expectedMoves[i].Steps.Count(), actualMoves[i].Steps.Count(), "Correct number of steps within move");
                for (var j = 0; j < expectedMoves[i].Steps.Count(); ++j)
                {
                    Assert.AreEqual(expectedMoves[i].Steps.ElementAt(j).Direction, actualMoves[i].Steps.ElementAt(j).Direction, "Correct direction");
                    Assert.AreEqual(expectedMoves[i].Steps.ElementAt(j).Jump, actualMoves[i].Steps.ElementAt(j).Jump, "Correct jump state.");
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
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.DOWN_TEAM_KING;
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = false }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.UP_TEAM_KING;
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = false }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.DOWN_TEAM_KING;
                pieces[5, 4] = Piece.DOWN_TEAM_KING;
                pieces[3, 4] = Piece.DOWN_TEAM_KING;
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.UP_TEAM_KING;
                pieces[5, 4] = Piece.UP_TEAM_KING;
                pieces[3, 4] = Piece.UP_TEAM_KING;
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.DOWN_TEAM_KING;
                pieces[5, 4] = Piece.DOWN_TEAM_KING;
                pieces[5, 2] = Piece.DOWN_TEAM_KING;
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = false }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.DOWN_TEAM_KING;
                pieces[5, 4] = Piece.UP_TEAM_KING;
                pieces[6, 5] = Piece.UP_TEAM_KING;
                pieces[5, 2] = Piece.DOWN_TEAM_KING;
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
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.DOWN_TEAM;
                pieces[5, 4] = Piece.UP_TEAM;
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.DOWN_TEAM;
                pieces[5, 4] = Piece.UP_TEAM_KING;
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.DOWN_TEAM;
                pieces[5, 4] = Piece.UP_TEAM;
                pieces[5, 2] = Piece.DOWN_TEAM;
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.DOWN_TEAM;
                pieces[5, 2] = Piece.UP_TEAM;
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = true}}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = false }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[2, 3] = Piece.DOWN_TEAM;
                pieces[3, 4] = Piece.UP_TEAM;
                pieces[5, 6] = Piece.UP_TEAM_KING;
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}}
                };
                CompareMoves(pieces, expectedMoves, 2, 3);
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[0, 1] = Piece.DOWN_TEAM;
                pieces[1, 2] = Piece.UP_TEAM;
                pieces[3, 4] = Piece.UP_TEAM_KING;
                pieces[5, 6] = Piece.UP_TEAM;
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
                var pieces = new Piece?[8, 8];
                pieces[0, 1] = Piece.DOWN_TEAM;
                pieces[1, 2] = Piece.UP_TEAM;
                pieces[3, 4] = Piece.UP_TEAM_KING;
                pieces[5, 4] = Piece.UP_TEAM;
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
                var pieces = new Piece?[8, 8];
                pieces[0, 1] = Piece.DOWN_TEAM;
                pieces[1, 2] = Piece.UP_TEAM;
                pieces[3, 2] = Piece.UP_TEAM_KING;
                pieces[5, 2] = Piece.UP_TEAM;
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
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.UP_TEAM;
                pieces[3, 4] = Piece.DOWN_TEAM;
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = true }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.UP_TEAM;
                pieces[3, 4] = Piece.DOWN_TEAM;
                pieces[1, 4] = Piece.DOWN_TEAM_KING;
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = true }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = true },
                                                                new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = true }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[2, 3] = Piece.DOWN_TEAM;
                pieces[3, 4] = Piece.UP_TEAM;
                pieces[5, 4] = Piece.UP_TEAM_KING;
                pieces[5, 6] = Piece.UP_TEAM_KING;
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
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.DOWN_TEAM_KING;
                pieces[5, 4] = Piece.UP_TEAM;
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.UP_TEAM_KING;
                pieces[5, 4] = Piece.DOWN_TEAM;
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.DOWN_TEAM_KING;
                pieces[5, 4] = Piece.UP_TEAM;
                pieces[3, 2] = Piece.UP_TEAM;
                var expectedMoves = new List<Move>() {
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = true }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                    new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}}
                };
                CompareMoves(pieces, expectedMoves);
            }
            {
                var pieces = new Piece?[8, 8];
                pieces[4, 3] = Piece.DOWN_TEAM_KING;
                pieces[5, 4] = Piece.UP_TEAM;
                pieces[5, 6] = Piece.UP_TEAM_KING;
                pieces[3, 6] = Piece.UP_TEAM;
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
                var pieces = new Piece?[8, 8];
                pieces[2, 3] = Piece.DOWN_TEAM_KING;
                pieces[3, 4] = Piece.UP_TEAM;
                pieces[3, 6] = Piece.UP_TEAM_KING;
                pieces[1, 6] = Piece.UP_TEAM;
                pieces[1, 4] = Piece.UP_TEAM;
                pieces[3, 2] = Piece.UP_TEAM;
                pieces[5, 2] = Piece.UP_TEAM;
                pieces[5, 4] = Piece.UP_TEAM;
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
                var legalMoveFinder = new LegalMoveFinder(new GameBoard(pieces));
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

        //TODO: This is testing more than one function.  Rewrite the test so that a failure in GetLegalMoves() will not cause this to fail.
        [TestMethod]
        public void Moves_GetLegalMovesByTeam()
        {
            var pieces = new Piece?[8, 8];
            pieces[2, 3] = Piece.DOWN_TEAM;
            pieces[7, 6] = Piece.DOWN_TEAM_KING;
            pieces[3, 4] = Piece.UP_TEAM;
            var moveFinder = new LegalMoveFinder(new GameBoard(pieces));
            var actual1 = moveFinder.GetLegalMoves(true);
            var expected1 = new List<PositionMoves>()
            {
                new PositionMoves() {
                    Row = 2, 
                    Column = 3,
                    Moves = new List<Move>() {
                        new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_LEFT, Jump = false }}},
                        new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.DOWN_RIGHT, Jump = true }}}
                    }
                },
                new PositionMoves() {
                    Row = 7, 
                    Column = 6,
                    Moves = new List<Move>() {
                        new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = false }}},
                        new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = false }}}
                    }
                },
            };
            ComparePositionMoves(pieces, expected1, actual1);
            var actual2 = moveFinder.GetLegalMoves(false);
            var expected2 = new List<PositionMoves>()
            {
                new PositionMoves() {
                    Row = 3,
                    Column = 4,
                    Moves = new List<Move>() {
                        new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_LEFT, Jump = true }}},
                        new Move() { Steps = new List<MoveStep>() { new MoveStep() { Direction = MoveDirection.UP_RIGHT, Jump = false }}}
                    }
                }
            };
            ComparePositionMoves(pieces, expected2, actual2);
        }

        private void ComparePositionMoves(Piece?[,] pieces, List<PositionMoves> expected, List<PositionMoves> actual)
        {
            expected = expected.OrderBy(p => p.Row).ThenBy(p => p.Column).ToList();
            actual = actual.OrderBy(p => p.Row).ThenBy(p => p.Column).ToList();
            Assert.AreEqual(expected.Count, actual.Count, "Number of Position Moves match.");
            for (var i = 0; i < expected.Count; ++i)
                CompareMoves(expected[i].Moves, actual[i].Moves, expected[i].Row, expected[i].Column);
        }
    }
}