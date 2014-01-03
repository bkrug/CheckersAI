﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckersAI.Models
{
    public class GameStatus
    {
        private Piece[,] _pieces;

        public GameStatus(Piece[,] pieces)
        {
            _pieces = pieces;
        }

        public bool? WinnerIsDownBoundTeam
        {
            get
            {
                return WinnerByElimination.HasValue
                    ? WinnerByElimination
                    : WinnerByGridlock.HasValue ? WinnerByGridlock : null;
            }
        }

        public bool? WinnerByElimination
        {
            get
            {
                int downPieces = 0;
                int upPieces = 0;
                int r = 0;
                int c = 0;
                while ((downPieces == 0 || upPieces == 0) && r <= LegalMoveFinder.MAX_POSITION)
                {
                    var piece = _pieces[r, c++];
                    if (piece != null)
                        if (piece.DownBoundTeam)
                            ++downPieces;
                        else
                            ++upPieces;
                    if (c == LegalMoveFinder.MAX_POSITION)
                    {
                        c = 0;
                        ++r;
                    }
                }
                if (upPieces == 0 && downPieces > 0)
                    return true;
                if (downPieces == 0 && upPieces > 0)
                    return false;
                return null;
            }
        }

        //SUSPECT: This is testing LegalMoveFinder() as well as one method.
        public bool? WinnerByGridlock
        {
            get
            {
                int downMoves = 0;
                int upMoves = 0;
                int r = 0;
                int c = 0;
                var moveFinder = new LegalMoveFinder(_pieces);
                while ((downMoves == 0 || upMoves == 0) && r <= LegalMoveFinder.MAX_POSITION)
                {
                    var piece = _pieces[r, c];
                    if (piece != null)
                        if (piece.DownBoundTeam)
                            downMoves += moveFinder.GetLegalMoves(r, c).Count();
                        else
                            upMoves += moveFinder.GetLegalMoves(r, c).Count();
                    ++c;
                    if (c == LegalMoveFinder.MAX_POSITION)
                    {
                        c = 0;
                        ++r;
                    }
                }
                if (upMoves == 0 && downMoves > 0)
                    return true;
                if (downMoves == 0 && upMoves > 0)
                    return false;
                return null;
            }
        }
    }
}