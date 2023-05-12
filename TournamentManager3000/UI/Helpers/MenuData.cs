using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentManager3000.Controllers;
using TournamentManager3000.UI.MainMenu;
using TournamentManager3000.UI.PlayerMenu;
using TournamentManager3000.UI.TournamentMenu;

namespace TournamentManager3000.UI.Helpers
{
    using MenuInput = List<string>;
    using SubmenuCommandDictionary = Dictionary<string, MenuAction>;
    using CommandDictionary = Dictionary<string, (Dictionary<string, MenuAction>? menuToSwitch, MenuAction action)>;
    public class MenuData
    {
        public CommandDictionary MainMenuCommands { get; }
        public SubmenuCommandDictionary TournamentMenuCommands { get; }
        public SubmenuCommandDictionary PlayerMenuCommands { get; }
        public SubmenuCommandDictionary ImportExportMenuCommands { get; }

        public string DummyMenuAction(MenuInput input) => "This is a dummy menu action";

        public string Exit(MenuInput input) => "See you next time!";
        public string Help(MenuInput input) => "Available commands ('<XXX>' means required attribute, '<XXX>?' means optional one):\n" +
                "'help' - shows this message\n" +
                "'exit' - exits the program\n" +
                "'player-menu' - opens the player menu\n" +
                "'tournament-menu' - opens the tournament menu\n" +
                "'import-export-menu' - opens the import/export menu\n";

        public MenuData(TournamentController tournamentController, PlayerController playerController, ImportExportController importExportController)
        {
            MainMenuCommands = new CommandDictionary()
            {
                {"help", (null, this.Help)},
                {"exit", (null, this.Exit)},
                {"player-menu", (PlayerMenuCommands, DummyMenuAction)},
                {"tournament-menu", (TournamentMenuCommands, DummyMenuAction)},
                {"import-export-menu", (ImportExportMenuCommands, DummyMenuAction)},
            };

            TournamentMenuCommands = new SubmenuCommandDictionary()
            {
                {"menu name", tournamentController.MenuName},
                {"help", tournamentController.Help},
                {"exit", tournamentController.Exit},
                {"create-tournament", tournamentController.CreateTournament},
                {"add-result", tournamentController.AddMatchResult},
                {"show-tournament", tournamentController.ShowTournament},
                {"list-tournaments", tournamentController.ListTournaments},
            };

            PlayerMenuCommands = new SubmenuCommandDictionary()
            {
                {"menu name", playerController.MenuName},
                {"help", playerController.Help},
                {"exit", playerController.Exit},
                {"create-player", playerController.CreatePlayer},
                {"show-player", playerController.ShowPlayer},
                {"list-players", playerController.ListPlayers},
            };

            ImportExportMenuCommands = new SubmenuCommandDictionary()
            {
                {"menu name", importExportController.MenuName},
                {"help", importExportController.Help},
                {"exit", importExportController.Exit},
                {"import-players", importExportController.ImportPlayers},
                {"export-players", importExportController.ExportPlayers},
                {"export-empty-tournament-schema", importExportController.ExportEmptyTournamentSchema},
                {"export-existing-tournament-schema", importExportController.ExportExistingTournamentSchema},
            };
        }
    }
}
