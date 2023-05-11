using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentManager3000.UI.MainMenu;
using TournamentManager3000.UI.PlayerMenu;
using TournamentManager3000.UI.TournamentMenu;

namespace TournamentManager3000.UI.Helpers
{
    public class MenuData
    {
        public readonly Dictionary<string, (AbstractMenu? menuToSwitch, MenuOperationsEnum operation)> CommandMapper;

        public MenuData(MainMenuExec mainMenu, TournamentMenuExec tournamentMenu, PlayerMenuExec playerMenu)
        {
            CommandMapper = new Dictionary<string, (AbstractMenu? followingMenu, MenuOperationsEnum operation)>
            {
                {"help", (null, MenuOperationsEnum.Help)},
                {"exit", (mainMenu, MenuOperationsEnum.Exit)},
                {"create-tournament", (tournamentMenu, MenuOperationsEnum.CreateTournament)},
                {"select-tournament", (tournamentMenu, MenuOperationsEnum.SelectTournament)},
                {"list-tournaments", (tournamentMenu, MenuOperationsEnum.ListTournaments)},
                {"create-player", (playerMenu, MenuOperationsEnum.CreatePlayer)},
                {"select-player", (playerMenu, MenuOperationsEnum.SelectPlayer)},
                {"delete-player", MenuOperationsEnum.DeletePlayer}

                {"list-players", MenuOperationsEnum.ListPlayers},
                {"delete-tournament", MenuOperationsEnum.DeleteTournament},

            };
        }

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
