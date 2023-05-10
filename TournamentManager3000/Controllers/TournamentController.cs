using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentManager3000.Controllers
{
    public class TournamentController
    {
        private readonly TournamentContext _tournamentContext;
        public TournamentController(TournamentContext tournamentContext)
        {
            _tournamentContext = tournamentContext;
        }
    }
}
