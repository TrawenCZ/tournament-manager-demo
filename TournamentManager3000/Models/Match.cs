using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentManager3000.Models
{
    internal class Match
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }
        public int Team1Id { get; set; }
        public int Team2Id { get; set; }
        public int TeamWinnerId { get; set; }
    }
}
