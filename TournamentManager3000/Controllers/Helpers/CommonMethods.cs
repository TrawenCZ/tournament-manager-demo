using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentManager3000.Models;

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


        public static string BuildTableFromDictionary(Dictionary<string, List<string>> data)
        {
            // Determine the number of rows and columns in the table
            int numRows = data.First().Value.Count;
            int numCols = data.Count;

            // Determine the maximum width of each column
            int[] columnWidths = new int[numCols];
            int i = 0;
            foreach (string columnName in data.Keys)
            {
                int maxColumnWidth = Math.Max(columnName.Length, data[columnName].Max(value => value.Length));
                columnWidths[i++] = maxColumnWidth;
            }

            // Build the table header
            StringBuilder tableBuilder = new StringBuilder();
            foreach (string columnName in data.Keys)
            {
                tableBuilder.Append(columnName.PadRight(columnWidths[i], ' '));
                tableBuilder.Append(" | ");
            }
            tableBuilder.Remove(tableBuilder.Length - 3, 3); // Remove the last separator and trailing space
            tableBuilder.AppendLine();

            // Build the separator row
            foreach (int columnWidth in columnWidths)
            {
                tableBuilder.Append(new string('-', columnWidth));
                tableBuilder.Append("-+-");
            }
            tableBuilder.Remove(tableBuilder.Length - 3, 3); // Remove the last separator and trailing hyphen
            tableBuilder.AppendLine();

            // Build the data rows
            for (int row = 0; row < numRows; row++)
            {
                foreach (string columnName in data.Keys)
                {
                    tableBuilder.Append(data[columnName][row].PadRight(columnWidths[i], ' '));
                    tableBuilder.Append(" | ");
                }
                tableBuilder.Remove(tableBuilder.Length - 3, 3); // Remove the last separator and trailing space
                tableBuilder.AppendLine();
            }

            return tableBuilder.ToString();
        }


        public static bool TryParsePlayer(string idOrNickname, TournamentContext tournamentContext, out Player player)
        {
            Player? playerOrDefault = null;
            player = new Player();
            if (int.TryParse(idOrNickname, out int id))
            {
                playerOrDefault = tournamentContext.Players.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
                if (playerOrDefault != null)
                {
                    player = playerOrDefault;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                playerOrDefault = tournamentContext.Players.FirstOrDefault(p => p.Nickname == idOrNickname && !p.IsDeleted);
                if (playerOrDefault != null)
                {
                    player = playerOrDefault;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

    }
}
