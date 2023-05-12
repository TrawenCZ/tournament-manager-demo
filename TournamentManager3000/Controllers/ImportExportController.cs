using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentManager3000.Controllers
{
    public class ImportExportController
    {
        private readonly TournamentContext _tournamentContext;
        public ImportExportController(TournamentContext tournamentContext) 
        {
            _tournamentContext = tournamentContext;
        }
    }
}
