using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentManager3000.Models
{
    public class Player : IEntity
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; } = false;

        public int Wins { get; set; } = 0;
        public int Losses { get; set; } = 0;
        public int MatchesPlayed { get; set; } = 0;

        //public List<Match> Matches { get; set; } = new List<Match>();
    }
}
