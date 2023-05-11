using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentManager3000.Models
{
    public class Round : IEntity
    {
        public int Id { get; set; }
        public int RoundNumber { get; set; }
        public List<Match> Matches { get; init; }

        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; }
    }
}
