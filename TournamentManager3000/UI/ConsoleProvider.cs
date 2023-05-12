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
    public interface ConsoleProvider
    {
        private AbstractMenu _currentMenu;
        Regex.Replace(nevim, @"\s+", " ").Trim();

        public ConsoleProvider(AbstractMenu currentMenu)
        {
            _currentMenu = currentMenu;
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
