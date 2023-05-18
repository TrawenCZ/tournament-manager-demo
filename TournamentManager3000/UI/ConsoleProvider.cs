using System.Text.RegularExpressions;
using TournamentManager3000.Controllers.Helpers;
using TournamentManager3000.UI.Helpers;

namespace TournamentManager3000.UI
{
    using CommandDictionary = Dictionary<string, Dictionary<string, MenuAction>>;
    using SubmenuCommandDictionary = Dictionary<string, MenuAction>;
    public class ConsoleProvider
    {
        private LoadingSpinner _loadingSpinner;         // asynchrounous loading spinning animation

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

            SubmenuCommandDictionary currentSubmenuData = data.TournamentMenuCommands;      // kinda like menu context
            CommandDictionary menuSwitches = data.MenuSwitches;                             // commands to switch these contexts
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
                        return;
                    }
                    if (command == "help")
                    {
                        if (inSubmenu) Console.WriteLine(menuNamesAndHelpMsgs[currentSubmenuData].HelpMsg);
                        else Console.WriteLine(data.MainMenuHelpMsg);
                        continue;
                    }

                    CancellationTokenSource ctsForLoading = new CancellationTokenSource();

                    if (inSubmenu)
                    {
                        if (currentSubmenuData.TryGetValue(command, out var subMenuAction)) taskForActions = Task.Run(() => subMenuAction!(argumentsToPass));
                        else taskForActions = Task.Run(() => $"Unrecognized action. {CommandHelper(command, currentSubmenuData.Keys.ToList())}\n\n" + menuNamesAndHelpMsgs[currentSubmenuData].HelpMsg);
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
                        taskForActions = Task.Run(() => $"Unrecognized action. {CommandHelper(command, menuSwitches.Keys.ToList())}\n\n" + data.MainMenuHelpMsg);
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
                    Console.WriteLine($"Do you want to exit program now? (Yes/No)");
                    var answer = Console.ReadLine();
                    if (answer?.ToLower() == "yes" || answer?.ToLower() == "y") shouldContinue = false;

                    if (inSubmenu)
                        {
                            currentSubmenuData["back"](filler);
                            inSubmenu = false;
                        }
                }
            }

            Console.WriteLine("See you next time!");
        }

        private string CommandHelper(string input, List<string> commands)
        {
            var output = commands.Select(c => new { Command = c, SimDistance = ComputeStringSimilarity(c, input) }).OrderBy(cs => cs.SimDistance).FirstOrDefault(cs => cs.SimDistance < 5);
            return output == null ? "" : $"Did you mean '{output.Command}'?";
        }

        private int ComputeStringSimilarity(string command, string input)         // computation of string similarity with Levenshtein distance algorithm, main part of it is from StackOverflow, I'm not that smart
        {
            if (string.IsNullOrEmpty(command))
            {
                if (string.IsNullOrEmpty(input))
                    return 0;
                return input.Length;
            }

            if (string.IsNullOrEmpty(input))
            {
                return command.Length;
            }

            int commandLen = command.Length;
            int inputLen = input.Length;
            int[,] distance = new int[commandLen + 1, inputLen + 1];

            for (int i = 0; i <= commandLen; distance[i, 0] = i++) ;
            for (int j = 1; j <= inputLen; distance[0, j] = j++) ;

            for (int i = 1; i <= commandLen; i++)
            {
                for (int j = 1; j <= inputLen; j++)
                {
                    int cost = (input[j - 1] == command[i - 1]) ? 0 : 1;
                    int min1 = distance[i - 1, j] + 1;
                    int min2 = distance[i, j - 1] + 1;
                    int min3 = distance[i - 1, j - 1] + cost;
                    distance[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return distance[commandLen, inputLen];
        }
    }
}
