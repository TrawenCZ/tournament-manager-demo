using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentManager3000.Controllers.Helpers
{
    public static class CommonMethods
    {
        public static bool CheckListLength(List<string> list, int minLength, int maxLength, out string message)
        {
            if (list.Count < minLength)
            {
                message = $"You have to enter at least {minLength} arguments.";
                return false;
            }
            else if (list.Count > maxLength)
            {
                message = $"You have to enter at most {maxLength} arguments.";
                return false;
            }
            else
            {
                message = "";
                return true;
            }
        }

        public static List<string> StringsToPadded(List<string> list)
        {
            int maxLength = list.Max(x => x.Length);
            List<string> paddedList = new List<string>();
            foreach (string item in list)
            {
                paddedList.Add(item.PadRight(maxLength));
            }
            return paddedList;
        }
    }
}
