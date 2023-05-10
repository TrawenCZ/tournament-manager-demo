using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentManager3000.UI.MainMenu
{
    public class MainMenuData
    {
        public readonly Dictionary<string, MainMenuOpEnum> commandMapper = new Dictionary<string, MainMenuOpEnum>
            {
                {"help", MainMenuOpEnum.Help},
                {"exit", MainMenuOpEnum.Exit},
                {"create-tournament", MainMenuOpEnum.CreateTournament},
                {"show-tournament", MainMenuOpEnum.ShowTournament},
                {"create-player", MainMenuOpEnum.CreatePlayer},
                {"show-player", MainMenuOpEnum.ShowPlayer}
            };

        private class Messages
        {
            public readonly string WELCOME_MSG = "Welcome to TournamentManager3000!\n" +
                    "Type 'help' to see available commands.\n";

            public readonly string HELP_MSG =  "Available commands ('<XXX>' means required attribute, '<XXX>?' means optional one):\n" +
                    "'help' - shows this message\n" +
                    "'exit' - exits the program\n" +
                    "'create-tournament <NAME> <DESCRIPTION>?' - creates a new tournament\n" +
                    "'show-tournament <ID>' - shows a tournament\n" +
                    "'list-tournaments' - lists all tournaments'\n" +
                    "'create-player <NICKNAME> <DESCRIPTION>?' - creates a new player\n" +
                    "'show-player <ID or NICKNAME>' - shows a player\n";

            public readonly string EXIT_MSG = "Exiting the program...";

            public readonly string CREATE_TOUR_MSG = "Creating a new tournament...";

            public readonly string SHOW_TOUR_MSG = "Showing a tournament...";

            public readonly string CREATE_PLAYER_MSG = "Creating a new player...";

            public readonly string SHOW_PLAYER_MSG = "Showing a player...";

        }
    }
}
