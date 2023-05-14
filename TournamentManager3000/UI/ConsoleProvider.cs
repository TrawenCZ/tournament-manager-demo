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
            bool changeInSubmenuAtTheEnd = false;
            string messageToPrint = "";
            List<string> filler = new List<string>();
            Task<string> taskForActions = Task.Run(() => "");

            SubmenuCommandDictionary currentSubmenuData = data.TournamentMenuCommands;
            CommandDictionary mainMenuData = data.MainMenuCommands;

            Console.WriteLine("Welcome to TournamentManager3000 - the best and most powerful tournament manager yet.\nLet's start with list of available commands in Main menu:\n\n" + mainMenuData["help"].action(filler));
            

            while (shouldContinue)
            {
                string currMenuName = inSubmenu ? currentSubmenuData["menu name"](filler) : "Main";    // sorry for this ugly trick, but it's effective
                Console.Write($"\n[{(inSubmenu ? currentSubmenuData["menu name"](filler) : "Main menu")}]> ");

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
                } else if (command == "back" && inSubmenu)
                {
                    shouldContinue = inSubmenu;
                    changeInSubmenuAtTheEnd = true;
                }

                CancellationTokenSource ctsForLoading = new CancellationTokenSource();

                if (inSubmenu)
                {
                    if (currentSubmenuData.TryGetValue(command, out var subMenuAction)) taskForActions = Task.Run(() => subMenuAction!(argumentsToPass));
                    else taskForActions = Task.Run(() => "Unrecognized action.\n\n" + currentSubmenuData["help"](filler));
                } else if (mainMenuData.TryGetValue(command, out var mainMenuAction))
                {
                    if (mainMenuAction.menuToSwitch != null)
                    {
                        currentSubmenuData = mainMenuAction.menuToSwitch;
                        currMenuName = currentSubmenuData["menu name"](filler);
                        taskForActions = Task.Run(() => $"Welcome to {currMenuName} menu.\n\n" + currentSubmenuData["help"](filler));
                        inSubmenu = true;
                    } else
                    {
                        taskForActions = Task.Run(() => mainMenuAction.action(argumentsToPass));
                    }
                } else
                {
                    taskForActions = Task.Run(() => "Unrecognized action.\n\n" + mainMenuData["help"].action(filler));
                }
                Task spinningAnimation = _loadingSpinner.Start(ctsForLoading.Token);
                messageToPrint = await taskForActions;
                ctsForLoading.Cancel();
                await spinningAnimation;

                if (changeInSubmenuAtTheEnd)
                {
                    inSubmenu = false;
                    changeInSubmenuAtTheEnd = false;
                }

                Console.WriteLine(messageToPrint);
            }
        }

        /*
        public List<string> ReadAndSplitLine(string message)
        {
            Console.WriteLine(message);
            string? inputLine = Console.ReadLine();
            if (inputLine == null) return new List<string>();
            return new List<string>(Regex.Replace(inputLine, @"\s+", " ").Trim().Split());
        }
        */
    }
}
