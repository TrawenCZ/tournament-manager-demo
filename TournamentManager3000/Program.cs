// See https://aka.ms/new-console-template for more information
using TournamentManager3000.Controllers;
using TournamentManager3000.Controllers.Helpers;
using TournamentManager3000.Data;
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
            // I left just two players in db, so that you can test functionality with empty storage and some records yourself. Nicknames were definitely not chosen for better grading.
            try
            {
                LoadingSpinner loadingSpinner = new LoadingSpinner();
                ConsoleProvider consoleProvider = new ConsoleProvider(loadingSpinner);
                TournamentCreator tournamentCreator = new TournamentCreator();
                using (var context = new TournamentContext())
                {
                    TournamentController tournamentController = new TournamentController(context, tournamentCreator, consoleProvider);
                    PlayerController playerController = new PlayerController(context);
                    ImportExportController importExportController = new ImportExportController(context, tournamentCreator);

                    MenuData menuData = new MenuData(tournamentController, playerController, importExportController);
                    await consoleProvider.CommunicateWithUser(menuData);
                }
            }
            catch (Exception ex)            // this should never happen, but just in case, I know Microsoft (with combination with my skills)
            {
                Console.WriteLine(ex);
                return;
            }
        }
    }
}