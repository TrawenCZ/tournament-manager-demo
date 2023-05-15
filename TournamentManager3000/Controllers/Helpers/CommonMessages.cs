using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TournamentManager3000.Controllers.Helpers
{
    public static class CommonMessages
    {
        public static readonly string HELP_OPTION = "'help' - shows this message\n";
        public static readonly string HELP_HEADER = "Available commands:\n";
        public static readonly string NUM_ARG = " argument must be a number.";
        public static readonly string ID_NOT_FOUND = " with given ID does not exist!";
        public static readonly string SUCC_DEL = " was successfuly deleted.";
        public static readonly string BACK_TO_MAIN = "'back' - returns to Main menu\n";
    }
}
