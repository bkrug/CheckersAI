using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CheckersAI.Models
{
    public class MovePlan
    {
        public Move Move { get; set; }
        public int Wins;
        public int Loses;
    }
}