using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CheckersAI.Models;

namespace CheckersAI.Controllers
{
    public class BoardController : Controller
    {
        public const string BOARD_KEY = "gameBoard";

        public ActionResult MovePiece(int row, int column, string move)
        {
            var jss= new JavaScriptSerializer();
            var moveObj = jss.Deserialize<Move>(move);
            var board = (GameBoard)Session[BOARD_KEY];
            if (board == null)
                return Json(new { success = false, error = "No game has been started." });
            board.MovePiece(row, column, moveObj);
            return Json(new { success = true, board });
        }

    }
}
