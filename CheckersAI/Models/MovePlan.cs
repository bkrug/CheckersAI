using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckersAI.Models
{
    public class MovePlan
    {
        public int StartRow;
        public int StartColumn;
        public Move Move { get; set; }
        public int Heuristic;
    }
}