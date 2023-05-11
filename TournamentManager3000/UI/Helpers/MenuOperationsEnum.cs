using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentManager3000.UI.Helpers
{
    public enum MenuOperationsEnum
    {
        // Main Menu
        CreateTournament,
        SelectTournament,
        ListTournaments,
        CreatePlayer,
        SelectPlayer,
        ListPlayers,
        DeletePlayer,
        DeleteTournament,

        // Tournament Creation Menu
        AddPlayers,
        AddMatchResult,

        // Tournament Info Menu
        ListMatches,
        ListPlayersInTournament,
        SelectTournamentWinner,
        SelectMatch,

        // Player Selection Menu
        EditNickname,
        EditDescription,

        // Common
        ShowStatistics,
        Help,
        Exit
    }
}
