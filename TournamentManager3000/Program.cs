// See https://aka.ms/new-console-template for more information
using System.Collections.Generic;
using TournamentManager3000.Controllers;
using TournamentManager3000.Data;
using TournamentManager3000.Models;
using TournamentManager3000.UI;
using TournamentManager3000.UI.Helpers;

namespace TournamentManager3000
{

    using MenuInput = List<string>;
    public delegate string MenuAction(MenuInput inputList);

    public class Program
    {
        public async static Task Main()
        {
            try
            {
                ConsoleProvider consoleProvider = new ConsoleProvider();
                using (var context = new TournamentContext())
                {
                    TournamentController tournamentController = new TournamentController(context, consoleProvider);
                    PlayerController playerController = new PlayerController(context);
                    ImportExportController importExportController = new ImportExportController(context);

                    MenuData menuData = new MenuData(tournamentController, playerController, importExportController);
                    await consoleProvider.CommunicateWithUser(menuData);
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }
        }
    }
}