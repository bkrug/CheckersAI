using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CheckersAI.Models;

namespace CheckersAI.Tests
{
    [TestClass]
    public class GameStatusTests
    {
        [TestMethod]
        public void Status_WinnerByElimination()
        {
            {
                var pieces = new Piece[8, 8];
                pieces[4, 3] = new Piece() { DownBoundTeam = true };
                pieces[5, 6] = new Piece() { DownBoundTeam = true };
                pieces[2, 1] = new Piece() { DownBoundTeam = true };
                var status1 = new GameStatus(pieces);
                Assert.AreEqual(true, status1.WinnerByElimination, "DownBoundTeam is calculated as winner.");
                pieces[7, 2] = new Piece() { DownBoundTeam = false };
                Assert.AreEqual(null, status1.WinnerByElimination, "No team is calculated as winner.");
            }
            {
                var pieces = new Piece[8, 8];
                var status1 = new GameStatus(pieces);
                pieces[4, 3] = new Piece() { DownBoundTeam = false };
                pieces[5, 6] = new Piece() { DownBoundTeam = false };
                pieces[2, 1] = new Piece() { DownBoundTeam = false };
                Assert.AreEqual(false, status1.WinnerByElimination, "DownBoundTeam is calculated as loser.");
            }
        }

        [TestMethod]
        public void Status_WinnerByDeadlock()
        {
            {
                var pieces = new Piece[8, 8];
                pieces[4, 3] = new Piece() { DownBoundTeam = true };
                pieces[5, 2] = new Piece() { DownBoundTeam = false };
                pieces[5, 4] = new Piece() { DownBoundTeam = false };
                pieces[6, 1] = new Piece() { DownBoundTeam = false };
                pieces[6, 5] = new Piece() { DownBoundTeam = false };
                var status1 = new GameStatus(pieces);
                Assert.AreEqual(false, status1.WinnerByGridlock, "UpBoundTeam is calculated as winner.");
                pieces[1, 2] = new Piece() { DownBoundTeam = true };
                Assert.AreEqual(null, status1.WinnerByElimination, "No team is calculated as winner.");
            }
            {
                var pieces = new Piece[8, 8];
                pieces[6, 3] = new Piece() { DownBoundTeam = false };
                pieces[5, 2] = new Piece() { DownBoundTeam = true };
                pieces[5, 4] = new Piece() { DownBoundTeam = true };
                pieces[4, 1] = new Piece() { DownBoundTeam = true };
                pieces[4, 5] = new Piece() { DownBoundTeam = true };
                var status1 = new GameStatus(pieces);
                Assert.AreEqual(true, status1.WinnerByGridlock, "UpBoundTeam is calculated as winner.");
            }
        }
    }
}
