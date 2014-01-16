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
        public const string FACTORY_KEY = "gameFactory";
        public const string TEAM_KEY = "playerTeam";

        private ISessionWrapper _session;
        private JavaScriptSerializer _jss = new JavaScriptSerializer();
        private IGameBoard _board { 
            get { return (IGameBoard)_session[BOARD_KEY]; }
        }

        public BoardController() {

        }

        public BoardController(ISessionWrapper session, IGameBoard board, IMovePlannerFactory plannerFactory)
        {
            _session = session;
            _session[BOARD_KEY] = board;
            _session[FACTORY_KEY] = plannerFactory;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (_session == null)
            {
                _session = new SessionWrapper(Session);
                _session[BOARD_KEY] = new GameBoard();
                _session[FACTORY_KEY] = MovePlannerFactory.Instance;
            }
        }

        public ActionResult Index()
        {
            return View("Game", _board);
        }

        [HttpPost]
        public ActionResult MovePiece(int row, int column, string move)
        {
            if (_board.Winner.HasValue)
                return Json(new { success = false, error = "Cannot make a move. The game is complete." });
            var moveObj = _jss.Deserialize<Move>(move);
            _board.MovePiece(row, column, moveObj);
            return Json(new { success = true, board = _board });
        }

        [HttpPost]
        public ActionResult GetComputerMove()
        {
            var factory = ((IMovePlannerFactory)_session[FACTORY_KEY]);
            var planner = factory.GetMovePlanner(_board);
            var movePlan = planner.GetNextMove(!((bool)_session[TEAM_KEY]));
            _board.MovePiece(movePlan.StartRow, movePlan.StartColumn, movePlan.Move);
            return Json(new { success = true, board = _board });
        }

        [HttpPost]
        public ActionResult StartGame(bool team)
        {
            _session[TEAM_KEY] = team;
            _board.Reset();
            return Json(new { success = true, _board });
        }

        public ActionResult GetPieceEnum()
        {
            var dict = new Dictionary<string, int>();
            foreach(var curPiece in Enum.GetValues(typeof(Piece)) )
            {
                dict.Add(curPiece.ToString(), (int)curPiece);
            }
            return Json(new { pieceDictionary = dict });
        }
    }

    public interface ISessionWrapper
    {
        object this[string index] { get; set; }
    }

    public class SessionWrapper : ISessionWrapper
    {
        private HttpSessionStateBase _session;
        private System.Web.SessionState.HttpSessionState httpSessionState;

        public object this[string index]
        {
            get { return _session[index]; }
            set
            {
                _session[index] = value;
            }
        }

        public SessionWrapper(HttpSessionStateBase session)
        {
            _session = session;
        }
    }
}
