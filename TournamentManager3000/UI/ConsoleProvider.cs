using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TournamentManager3000.UI.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TournamentManager3000.UI
{
    using SubmenuCommandDictionary = Dictionary<string, MenuAction>;
    using CommandDictionary = Dictionary<string, (Dictionary<string, MenuAction>? menuToSwitch, MenuAction action)>;
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
            string messageToPrint = "";
            List<string> filler = new List<string>();
            Task<string> taskForActions = Task.Run(() => "");

            SubmenuCommandDictionary currentSubmenuData = data.TournamentMenuCommands;
            CommandDictionary mainMenuData = data.MainMenuCommands;

            MenuAction subMenuAction = currentSubmenuData["exit"];
            (SubmenuCommandDictionary? menuToSwitch, MenuAction action) mainMenuAction = mainMenuData["exit"];


            while (shouldContinue)
            {
                string currMenuName = inSubmenu ? currentSubmenuData["menu name"](filler) : "Main";    // sorry for this ugly trick, but it's effective
                Console.Write($"[{currMenuName} menu]> ");

                var input = Console.ReadLine();
                if (input == null) continue;
                List<string> splittedInput = new List<string>(Regex.Replace(input, @"\s+", " ").Trim().Split());
                if (splittedInput.Count == 0)
                {
                    Console.WriteLine("Please provide a command from command list.");
                    continue;
                }

                string command = splittedInput[0].ToLower();
                List<string> argumentsToPass = splittedInput.Skip(1).ToList();

                if (command == "exit")
                {
                    Console.WriteLine($"Are you sure you want {(inSubmenu ? currMenuName + "menu" : "program")}? (Yes/No)");
                    var answer = Console.ReadLine();
                    if (answer?.ToLower() != "yes" && answer?.ToLower() != "y") continue;
                    inSubmenu = false;
                    shouldContinue = false;
                }

                CancellationTokenSource ctsForLoading = new CancellationTokenSource();

                if (inSubmenu)
                {
                    if (!currentSubmenuData.TryGetValue(command, out subMenuAction!)) messageToPrint = "Unrecognized action.\n" + currentSubmenuData["help"](filler);
                    else taskForActions = Task.Run(() => currentSubmenuData[command](argumentsToPass));
                } else
                {
                    if (!mainMenuData.TryGetValue(command, out mainMenuAction)) messageToPrint = "Unrecognized action.\n" + mainMenuData["help"].action(filler);
                    else if (mainMenuAction.menuToSwitch != null)
                    {
                        currentSubmenuData = mainMenuAction.menuToSwitch;
                        currMenuName = currentSubmenuData["menu name"](filler);
                        messageToPrint = currentSubmenuData["help"](filler);
                        inSubmenu = true;
                    } else {
                        taskForActions = Task.Run(() => mainMenuAction.action(argumentsToPass));
                    }
                }
                Task spinningAnimation = _loadingSpinner.Start(ctsForLoading.Token);
                messageToPrint = await taskForActions;
                ctsForLoading.Cancel();
                await spinningAnimation;

                Console.WriteLine(messageToPrint);
            }
        }

        public List<string> ReadAndSplitLine(string message)
        {
            Console.WriteLine(message);
            string? inputLine = Console.ReadLine();
            if (inputLine == null) return new List<string>();
            return new List<string>(Regex.Replace(inputLine, @"\s+", " ").Trim().Split());
        }
    }
}
