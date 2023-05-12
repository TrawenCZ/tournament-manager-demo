// See https://aka.ms/new-console-template for more information
using System.Collections.Generic;
using TournamentManager3000.Controllers;
using TournamentManager3000.Models;
using TournamentManager3000.UI;
using TournamentManager3000.UI.Helpers;

namespace TournamentManager3000 {

    using MenuInput = List<string>;
    public delegate string MenuAction(MenuInput inputList);

    public class Program
    {
        public async static Task Main()
        {
            TournamentContext context = new TournamentContext();
            ConsoleProvider consoleProvider = new ConsoleProvider();
            TournamentController tournamentController = new TournamentController(context, consoleProvider);
            PlayerController playerController = new PlayerController(context);
            ImportExportController importExportController = new ImportExportController(context);

            MenuData menuData = new MenuData(tournamentController, playerController, importExportController);
            await consoleProvider.CommunicateWithUser(menuData);
        }
    }
}