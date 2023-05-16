using TournamentManager3000.Controllers;
using TournamentManager3000.Controllers.Helpers;

namespace TournamentManager3000.UI.Helpers
{
    using CommandDictionary = Dictionary<string, Dictionary<string, MenuAction>>;
    using MenuInput = List<string>;
    using SubmenuCommandDictionary = Dictionary<string, MenuAction>;
    public class MenuData
    {
        public CommandDictionary MenuSwitches { get; }
        public SubmenuCommandDictionary TournamentMenuCommands { get; }
        public SubmenuCommandDictionary PlayerMenuCommands { get; }
        public SubmenuCommandDictionary ImportExportMenuCommands { get; }
        public Dictionary<SubmenuCommandDictionary, (string MenuName, string HelpMsg)>  MenuNamesAndHelpMsgs { get; }

        public readonly string MainMenuHelpMsg = CommonMessages.HELP_HEADER +
                CommonMessages.HELP_OPTION +
                "'player-menu' - opens the Player menu\n" +
                "'tournament-menu' - opens the Tournament menu\n" +
                "'import-export-menu' - opens the Import/Export menu\n" +
                "'exit' - exits the program\n";

        public MenuData(TournamentController tournamentController, PlayerController playerController, ImportExportController importExportController)
        {

            TournamentMenuCommands = new SubmenuCommandDictionary()
            {
                {"menu name", tournamentController.MenuName},
                {"help", tournamentController.Help},
                {"back", tournamentController.Back},
                {"create-tournament", tournamentController.Create},
                {"add-result", tournamentController.SetMatchWinner},
                {"show-tournament", tournamentController.ShowTournament},
                {"show-round", tournamentController.ShowRound},
                {"list-tournaments", tournamentController.ListTournaments},
                {"delete-tournament", tournamentController.DeleteTournament}
            };

            PlayerMenuCommands = new SubmenuCommandDictionary()
            {
                {"menu name", playerController.MenuName},
                {"help", playerController.Help},
                {"back", playerController.Back},
                {"create-player", playerController.CreatePlayer},
                {"show-player", playerController.ShowPlayer},
                {"list-players", playerController.ListPlayers},
                {"delete-player", playerController.DeletePlayer}
            };

            ImportExportMenuCommands = new SubmenuCommandDictionary()
            {
                {"menu name", importExportController.MenuName},
                {"help", importExportController.Help},
                {"back", importExportController.Back},
                {"import-players", importExportController.ImportPlayers},
                {"export-players", importExportController.ExportPlayers},
                {"export-tournament", importExportController.ExportTournamentSchema},
                {"export-empty-tournament", importExportController.ExportEmptyTournamentSchema},
            };

            MenuSwitches = new CommandDictionary()
            {
                {"player-menu", PlayerMenuCommands},
                {"tournament-menu", TournamentMenuCommands},
                {"import-export-menu", ImportExportMenuCommands},
            };

            MenuNamesAndHelpMsgs = new Dictionary<SubmenuCommandDictionary, (string MenuName, string HelpMsg)>
            {
                {TournamentMenuCommands, 
                    (MenuName: "Tournament menu", 
                     HelpMsg: CommonMessages.HELP_HEADER +
                              CommonMessages.HELP_OPTION +
                              "'create-tournament <NAME> <list of player ID/nicknames>' - creates new tournament with given players specified by their ID or nickname (separate with spaces)\n" +
                              "'add-result <MATCH NUM> <WINNER ID or WINNER NICKNAME>' - sets a winner for a given match (from current tournament)\n" +
                              "'show-tournament <ID>' - shows details about given tournament\n" +
                              "'show-round <TOURNAMENT ID> <ROUND NUM>' - shows deatils about selected round from given tournament\n" +
                              "'list-tournaments' - lists all stored tournaments\n" +
                              "'delete-tournament <ID>' - deletes tournament with given ID" +
                              CommonMessages.BACK_TO_MAIN) 
                },
                {PlayerMenuCommands, 
                    (MenuName: "Player menu", 
                     HelpMsg: CommonMessages.HELP_HEADER +
                              CommonMessages.HELP_OPTION +
                              "'create-player <NICKNAME>' - creates a player with given unique nickname\n" +
                              "'show-player <ID or NICKNAME>' - shows details about player with given ID/nickname\n" +
                              "'list-players' - lists all stored players\n" +
                              "'delete-player <ID or NICKNAME>' - deletes player with given ID/nickname\n" +
                              CommonMessages.BACK_TO_MAIN)
                },
                {ImportExportMenuCommands, 
                    (MenuName: "Import/Export menu",
                     HelpMsg: "Please note that FILEPATH argument cannot contain any spaces. " + CommonMessages.HELP_HEADER +
                              CommonMessages.HELP_OPTION +
                              "'import-players <FILEPATH>' - imports players from given file in JSON format\n" +
                              "'export-players <FILEPATH>' - exports stored players to given directory or file in JSON format\n" +
                              "'export-tournament <FILEPATH> <TOURNAMENT ID>' - exports filled schema to given directory TXT file\n" +
                              "'export-empty-tournament <FILEPATH> <TOURNAMENT NAME> <list of PLAYER IDs or NICKNAMEs>' - exports empty schema to given directory or TXT file\n" +
                              CommonMessages.BACK_TO_MAIN) 
                }
            };
        }
    }
}
