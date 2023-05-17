using System.Text.RegularExpressions;
using TournamentManager3000.Controllers.Helpers;
using TournamentManager3000.UI.Helpers;

namespace TournamentManager3000.UI
{
    using CommandDictionary = Dictionary<string, Dictionary<string, MenuAction>>;
    using SubmenuCommandDictionary = Dictionary<string, MenuAction>;
    public class ConsoleProvider
    {
        private LoadingSpinner _loadingSpinner;

        public ConsoleProvider()
        {
            _loadingSpinner = new LoadingSpinner();
        }

        public async Task CommunicateWithUser(MenuData data)
        {
            bool shouldContinue = true;
            bool inSubmenu = false;
            string messageToPrint;
            List<string> filler = new List<string>();
            Task<string> taskForActions;

            SubmenuCommandDictionary currentSubmenuData = data.TournamentMenuCommands;
            CommandDictionary menuSwitches = data.MenuSwitches;
            Dictionary<SubmenuCommandDictionary, (string MenuName, string HelpMsg)> menuNamesAndHelpMsgs = data.MenuNamesAndHelpMsgs;

            Console.WriteLine("Welcome to TournamentManager3000 - the best and most powerful tournament manager yet.\nLet's start with list of available commands in Main menu:\n\n" + data.MainMenuHelpMsg);


            while (shouldContinue)
            {
                try
                {
                    string currMenuName = inSubmenu ? menuNamesAndHelpMsgs[currentSubmenuData].MenuName : "Main menu";
                    Console.Write($"\n[{currMenuName}]> ");

                    var input = Console.ReadLine();

                    if (input == null) continue;
                    List<string> splittedInput = new List<string>(Regex.Replace(input, @"\s+", " ").Trim().Split());
                    if (splittedInput.Count == 0)
                    {
                        Console.WriteLine("Please provide a command from command list.");
                        continue;
                    }

                    Console.WriteLine();
                    string command = splittedInput[0].ToLower();
                    List<string> argumentsToPass = splittedInput.Skip(1).ToList();

                    if (command == "exit" && !inSubmenu)
                    {
                        Console.WriteLine($"Are you sure you want to exit program? (Yes/No)");
                        var answer = Console.ReadLine();
                        if (answer?.ToLower() != "yes" && answer?.ToLower() != "y") continue;
                        Console.WriteLine("See you next time!");
                        return;
                    }
                    if (command == "help" && !inSubmenu)
                    {
                        Console.WriteLine(data.MainMenuHelpMsg);
                        continue;
                    }

                    CancellationTokenSource ctsForLoading = new CancellationTokenSource();

                    if (inSubmenu)
                    {
                        if (currentSubmenuData.TryGetValue(command, out var subMenuAction)) taskForActions = Task.Run(() => subMenuAction!(argumentsToPass));
                        else taskForActions = Task.Run(() => $"Unrecognized action. {CommonMethods.CommandHelper(command, currentSubmenuData.Keys.ToList())}\n\n" + menuNamesAndHelpMsgs[currentSubmenuData].HelpMsg);
                    }
                    else if (menuSwitches.TryGetValue(command, out var mainMenuAction))
                    {
                        currentSubmenuData = mainMenuAction;
                        currMenuName = menuNamesAndHelpMsgs[currentSubmenuData].MenuName;
                        taskForActions = Task.Run(() => $"Welcome to {currMenuName}.\n\n" + menuNamesAndHelpMsgs[currentSubmenuData].HelpMsg);
                        inSubmenu = true;
                    }
                    else
                    {
                        taskForActions = Task.Run(() => $"Unrecognized action. {CommonMethods.CommandHelper(command, menuSwitches.Keys.ToList())}\n\n" + data.MainMenuHelpMsg);
                    }
                    Task spinningAnimation = _loadingSpinner.Start(ctsForLoading.Token);
                    messageToPrint = await taskForActions;
                    ctsForLoading.Cancel();
                    await spinningAnimation;

                    if (command == "back" && inSubmenu)
                    {
                        inSubmenu = false;
                    }

                    Console.WriteLine(messageToPrint);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception occured during runtime:\n" + e.Message);
                    if (inSubmenu)
                    {
                        currentSubmenuData["back"](filler);
                        inSubmenu = false;
                    }
                }
            }
        }
    }
}
