using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CheckersAI.Models;

namespace CheckersAI.Tests
{
    [TestClass]
    public class BoardPositionTests
    {
        //[TestMethod]
        //public void Position_Legal()
        //{
        //    var i = 0;
        //    var p1 = new BoardPosition(0, 1);
        //    Assert.IsTrue(true, "Position " + (++i) + " is valid.");
        //    var p2 = new BoardPosition(0, 7);
        //    Assert.IsTrue(true, "Position " + (++i) + " is valid.");
        //    var p3 = new BoardPosition(0, 3);
        //    Assert.IsTrue(true, "Position " + (++i) + " is valid.");
        //    var p4 = new BoardPosition(1, 2);
        //    Assert.IsTrue(true, "Position " + (++i) + " is valid.");
        //    var p5 = new BoardPosition(1, 6);
        //    Assert.IsTrue(true, "Position " + (++i) + " is valid.");
        //    var p6 = new BoardPosition(2, 5);
        //    Assert.IsTrue(true, "Position " + (++i) + " is valid.");
        //    var p7 = new BoardPosition(2, 7);
        //    Assert.IsTrue(true, "Position " + (++i) + " is valid.");
        //    var p8 = new BoardPosition(3, 4);
        //    Assert.IsTrue(true, "Position " + (++i) + " is valid.");
        //    var p9 = new BoardPosition(6, 3);
        //    Assert.IsTrue(true, "Position " + (++i) + " is valid.");
        //    var p10 = new BoardPosition(6, 7);
        //    Assert.IsTrue(true, "Position " + (++i) + " is valid.");
        //    var p11 = new BoardPosition(7, 2);
        //    Assert.IsTrue(true, "Position " + (++i) + " is valid.");
        //}

        //[TestMethod]
        //public void Position_NotLegal() 
        //{
        //    var offboard = new[] { new[] { 0, -4 }, new[] { -9, 3 }, new[] { 5, 9 }, new[] { 8, 4 }, new[] { -2, -4 }, new[] { -1, 9 }, new[] { 8, -4 } };
        //    var onboard = new[] { new[] { 0, 4 }, new[] { 3, 5 }, new[] { 7, 5 }, new[] { 6, 2 } };

        //    for (var i = 0; i < offboard.Length; ++i) 
        //    {
        //        try
        //        {
        //            var p = new BoardPosition(offboard[i][0], offboard[i][1]);
        //            Assert.Fail("Board Position " + offboard[i][0] + ", " + offboard[i][1] + " should fail since it is off the board");
        //        }
        //        catch (OffBoardException ex)
        //        {
        //            Assert.IsTrue(true, "Board Position " + offboard[i][0] + ", " + offboard[i][1] + " fails since it is off the board");
        //        }
        //        catch (ApplicationException ex)
        //        {
        //            Assert.Fail("When creating Board Position " + offboard[i][0] + ", " + offboard[i][1] + " wrong exception occurred.");
        //        }
        //    }
        //    for (var i = 0; i < onboard.Length; ++i)
        //    {
        //        try
        //        {
        //            var p = new BoardPosition(onboard[i][0], onboard[i][1]);
        //            Assert.Fail("Board Position " + onboard[i][0] + ", " + onboard[i][1] + " should fail since it is invalid.");
        //        }
        //        catch (InvalidPositionException ex)
        //        {
        //            Assert.IsTrue(true, "Board Position " + offboard[i][0] + ", " + offboard[i][1] + " fails since it is invalid.");
        //        }
        //        catch (ApplicationException ex)
        //        {
        //            Assert.Fail("When creating Board Position " + offboard[i][0] + ", " + offboard[i][1] + " wrong exception occurred.");
        //        }
        //    }
        //}
    }
}
