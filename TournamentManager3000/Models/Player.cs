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

        public int Wins { get; private set; } = 0;

        public void AddWin() => Wins++;

    }
}
