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
        public Team Team1 { get; set; }
        public Team Team2 { get; set; }
        public Team? Winner { get; set; }
    }
}
