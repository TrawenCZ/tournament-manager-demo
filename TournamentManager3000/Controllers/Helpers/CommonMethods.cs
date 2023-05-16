using System.Text;
using TournamentManager3000.Data;
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
            message = "";
            return true;
        }


        public static string BuildTableFromDictionary(Dictionary<string, List<string>> data)
        {
            int numRows = data.First().Value.Count;
            int numCols = data.Count;


            int[] columnWidths = new int[numCols];
            int i = 0;
            foreach (string columnName in data.Keys)
            {
                int maxColumnWidth = Math.Max(columnName.Length, data[columnName].Max(value => value.Length));
                columnWidths[i++] = maxColumnWidth;
            }


            StringBuilder tableBuilder = new StringBuilder();
            var colNames = data.Keys.ToList();
            for (int j = 0; j < colNames.Count; j++)
            {
                tableBuilder.Append(colNames[j].PadRight(columnWidths[j]));
                tableBuilder.Append(" | ");
            }
            tableBuilder.Remove(tableBuilder.Length - 3, 3);
            tableBuilder.AppendLine();

            foreach (int columnWidth in columnWidths)
            {
                tableBuilder.Append(new string('-', columnWidth));
                tableBuilder.Append("-+-");
            }
            tableBuilder.Remove(tableBuilder.Length - 3, 3);
            tableBuilder.AppendLine();

            for (int row = 0; row < numRows; row++)
            {
                for (int j = 0; j < colNames.Count; j++)
                {
                    var colName = colNames[j];
                    tableBuilder.Append(data[colName][row].PadRight(columnWidths[j]));
                    tableBuilder.Append(" | ");
                }
                tableBuilder.Remove(tableBuilder.Length - 3, 3);
                tableBuilder.AppendLine();
            }

            return tableBuilder.ToString();
        }


        public static bool TryParsePlayer(string idOrNickname, TournamentContext tournamentContext, out Player player)
        {
            Player? playerOrDefault;
            player = new Player();
            if (int.TryParse(idOrNickname, out int id))
            {
                playerOrDefault = tournamentContext.Players.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
                if (playerOrDefault != null)
                {
                    player = playerOrDefault;
                    return true;
                }
                return false;
            }
            playerOrDefault = tournamentContext.Players.FirstOrDefault(p => p.Nickname == idOrNickname && !p.IsDeleted);
            if (playerOrDefault != null)
            {
                player = playerOrDefault;
                return true;
            }
            return false;
        }

        public static string ListPlayers(List<Player> players)
        {
            return BuildTableFromDictionary(new Dictionary<string, List<string>>()
            {
                {"ID", players.Select(p => p.Id.ToString()).ToList() },
                {"Nickname", players.Select(p => p.Nickname).ToList() },
                {"Total Wins", players.Select(p => p.Wins.ToString()).ToList() },
                {"Total Losses", players.Select(p => p.Losses.ToString()).ToList() },
                {"Tournament Wins", players.Select(p => p.TournamentWins.ToString()).ToList() },
                {"Matches played", players.Select(p => p.MatchesPlayed.ToString()).ToList() },
                {"Win / Loss ratio", players.Select(p => p.Losses == 0 ? "No losses yet" : (p.Wins / p.Losses).ToString()).ToList() },
                {"Win / Match ratio", players.Select(p => p.MatchesPlayed == 0 ? "No matches played yet" : (p.Wins / p.MatchesPlayed).ToString()).ToList() },
            });
        }


        public static string CommandHelper(string input, List<string> commands)
        {
            var output = commands.Select(c => new { Command = c, SimDistance = ComputeStringSimilarity(c, input) }).OrderBy(cs => cs.SimDistance).FirstOrDefault(cs => cs.SimDistance < 5);
            return output == null ? "" : $"Did you mean '{output.Command}'?";
        }

        private static int ComputeStringSimilarity(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (string.IsNullOrEmpty(t))
                    return 0;
                return t.Length;
            }

            if (string.IsNullOrEmpty(t))
            {
                return s.Length;
            }

            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 1; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    int min1 = d[i - 1, j] + 1;
                    int min2 = d[i, j - 1] + 1;
                    int min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return d[n, m];
        }
    }
}
