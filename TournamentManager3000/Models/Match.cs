using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentManager3000.Models
{
    public class Match : IEntity
    {
        public int Id { get; set; }
        public int Player1Id { get; set; }
        public Player Player1 { get; set; }
        public int? Player2Id { get; set; }
        public Player? Player2 { get; set; }

        public int? WinnerId { get; set; }
        public Player? Winner { get; set; }

        public int RoundId { get; set; }
        public Round Round { get; set; }
    }
}
