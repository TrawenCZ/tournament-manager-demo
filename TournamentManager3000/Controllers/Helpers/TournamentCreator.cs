using Microsoft.EntityFrameworkCore;
using System.Text;
using TournamentManager3000.Data;
using TournamentManager3000.Models;

namespace TournamentManager3000.Controllers.Helpers
{
    public class TournamentCreator
    {
        private readonly Player _dummyPlayer = new Player() { Nickname = "Dummy" };     // Dummy player used to fill number of players to power of 2.



        public Tournament CreateTournamentAndFirstRound(List<Player> players, string tournamentName)
        {
            RandomInsertDummies(players);
            Tournament newTournament = new Tournament() { Name = tournamentName };

            StartNewRound(newTournament, players);
            return newTournament;
        }


        public Round StartNewRound(Tournament tournament, List<Player>? players = null)
        {
            List<Match> newMatches = new List<Match>();
            Match newMatch;
            if (players != null)
            {
                for (int i = 0; i < players.Count; i += 2)
                {
                    var nullOrPlayer = (players[i + 1] == _dummyPlayer ? null : players[i + 1]);
                    newMatch = new Match() { Player1 = players[i], Player2 = nullOrPlayer };
                    if (nullOrPlayer == null) newMatch.Winner = players[i];
                    newMatches.Add(newMatch);
                }
            }
            else
            {
                List<Match> currRoundMatches = tournament.Rounds.Last().Matches;
                for (int i = 0; i < currRoundMatches.Count; i += 2)
                {
                    newMatch = new Match() { Player1 = currRoundMatches[i].Winner!, Player2 = currRoundMatches[i + 1].Winner! };
                    newMatches.Add(newMatch);
                }
            }
            var round = new Round() { Matches = newMatches, RoundNumber = tournament.Rounds.Count + 1 };
            tournament.Rounds.Add(round);
            return round;
        }


        public bool TryLoadPlayers(List<string> idOrNicknameList, TournamentContext tournamentContext, out List<Player> players)
        {
            players = new List<Player>();
            foreach (var idOrNickname in idOrNicknameList)
            {
                if (CommonMethods.TryParsePlayer(idOrNickname, tournamentContext, out Player player))
                {
                    players.Add(player);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }


        private void RandomInsertDummies(List<Player> players)
        {
            int numOfPlayers = players.Count;

            // closest power of 2 higher than numOfPlayers
            int numOfDummyPlayers = (int)Math.Pow(2, Math.Ceiling(Math.Log(numOfPlayers, 2))) - numOfPlayers;

            Shuffle(players);
            for (int i = 1; i <= numOfDummyPlayers * 2; i += 2)
            {
                players.Insert(i, _dummyPlayer);
            }
        }


        private void Shuffle(List<Player> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Player value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public bool ContainsDuplicateByNickname(List<Player> players, out string duplicateNickname)
        {
            duplicateNickname = "";
            var duplicates = players.GroupBy(x => x.Nickname)
                .Where(g => g.Count() > 1)
                .Select(y => y.Key)
                .ToList();
            if (duplicates.Count > 0)
            {
                duplicateNickname = duplicates[0];
                return true;
            }
            return false;
        }


        public bool ContainsDuplicate(List<Player> players, out Player duplicate)
        {
            duplicate = new Player();
            var duplicates = players.GroupBy(x => x)
                .Where(g => g.Count() > 1)
                .Select(y => y.Key)
                .ToList();
            if (duplicates.Count > 0)
            {
                duplicate = duplicates[0];
                return true;
            }
            return false;
        }


        public bool TryParseTournament(int tournamentId, TournamentContext tournamentContext, out Tournament tournament)
        {
            tournament = new Tournament();
            var tournamentLoaded = tournamentContext.Tournaments
                .Include(t => t.Rounds)
                    .ThenInclude(r => r.Matches)
                        .ThenInclude(m => m.Player1)
                .Include(t => t.Rounds)
                    .ThenInclude(r => r.Matches)
                        .ThenInclude(m => m.Player2)
                 .FirstOrDefault(t => t.Id == tournamentId);
            if (tournamentLoaded != null)
            {
                tournament = tournamentLoaded;
                return true;
            }
            else return false;
        }


        private string FormatName(string name, int maxNameLength)
        {
            return $"[ {name} ]" + new string('-', maxNameLength - name.Length);
        }


        private List<string> RoundToSchema(Round? round, Round? prevRound, int maxNameLength, List<int> prevMiddles, out List<int> newMiddles)
        {
            // well, this one is complicated, but it gets the job done
            List<string> output = new List<string>();
            int horizontalLineLength = 5;
            int writeLength = maxNameLength + 4 + horizontalLineLength; // 4 for brackets and spaces around name
            int totalLength = writeLength + horizontalLineLength - 1;

            string endFillament = new string(' ', totalLength - writeLength);
            string horizontalLine = new string('-', horizontalLineLength - 1);
            string regHorizontalLine = horizontalLine + "+" + endFillament;
            string endingHorizontalLine = new string(' ', writeLength - 1) + "+" + horizontalLine;

            string verticalLine = new string(' ', writeLength - 1) + "|" + endFillament;
            string emptyLine = new string(' ', totalLength);

            newMiddles = new List<int>();
            string? fillement = null;
            if (round == null && prevRound == null) fillement = new string('_', maxNameLength);

            for (int i = 0; i < prevMiddles[0]; i++) output.Add(string.Empty);
            if (prevMiddles.Count == 1)
            {
                output.Add($"{{ {(fillement == null ? prevRound!.Matches.First().Winner!.Nickname : fillement)} }}");
                return output;
            }

            for (int i = 0; i < prevMiddles.Count; i += 2)
            {
                if (i > 0) for (int j = 0; j < prevMiddles[i] - prevMiddles[i - 1] - 1; j++) output.Add(emptyLine);

                if (fillement != null)
                {
                    output.Add(FormatName(fillement, maxNameLength) + regHorizontalLine);
                }
                else if (prevRound == null)
                {
                    output.Add(FormatName(round!.Matches[i / 2].Player1.Nickname, maxNameLength) + regHorizontalLine);
                }
                else
                {
                    output.Add(FormatName(prevRound!.Matches[i].Winner!.Nickname, maxNameLength) + regHorizontalLine);
                }

                var newGap = prevMiddles[i + 1] - prevMiddles[i];
                var newMiddle = newGap / 2;
                newMiddles.Add(prevMiddles[i] + newMiddle);
                var verticalLineCount = newMiddle - 1;

                for (int j = 0; j < verticalLineCount; j++) output.Add(verticalLine);
                output.Add(endingHorizontalLine);
                for (int j = 0; j < verticalLineCount; j++) output.Add(verticalLine);

                if (fillement != null)
                {
                    output.Add(FormatName(fillement, maxNameLength) + regHorizontalLine);
                }
                else if (prevRound == null)
                {
                    output.Add(FormatName(round!.Matches[i / 2].Player2 != null ? round!.Matches[i / 2].Player2!.Nickname : "Auto Advance", maxNameLength) + regHorizontalLine);
                }
                else
                {
                    output.Add(FormatName(prevRound!.Matches[i + 1].Winner!.Nickname, maxNameLength) + regHorizontalLine);
                }
            }

            return output;
        }


        private int MaxWinnerNameLengthInRound(Round round)
        {
            return round.Matches.Select(m => m.Winner!.Nickname.Length).Max();
        }



        public string TournamentToSchema(Tournament tournament, bool isEmpty)
        {
            List<int> newMiddles = new List<int>();
            int counter = 0;
            for (int i = 0; i < tournament.Rounds.First().Matches.Count * 2; i++)
            {
                newMiddles.Add(counter);
                counter += (i % 2 == 0 ? 4 : 2);
            }
            int maxNameLength = tournament.Rounds.First().Matches.Select(m => Math.Max(m.Player1.Nickname.Length, m.Player2 != null ? m.Player2.Nickname.Length : "Auto Advance".Length)).Max();
            List<StringBuilder> schemaBuilder = RoundToSchema(tournament.Rounds.First(), null, maxNameLength, new List<int>(newMiddles), out newMiddles).Select(s => new StringBuilder(s)).ToList();

            int indexer = 1;
            List<string> currRound = new List<string>();
            while (newMiddles.Count != 0)
            {
                if (isEmpty)
                {
                    currRound = RoundToSchema(null, null, maxNameLength, new List<int>(newMiddles), out newMiddles);
                }
                else if (tournament.Rounds.Count != 1 && tournament.Rounds.Count - 1 != indexer)
                {
                    currRound = RoundToSchema(tournament.Rounds[indexer],
                        tournament.Rounds[indexer - 1],
                        MaxWinnerNameLengthInRound(tournament.Rounds[indexer - 1]),
                        new List<int>(newMiddles),
                        out newMiddles);
                    indexer++;
                }
                else
                {
                    currRound = RoundToSchema(null, tournament.Rounds[indexer - 1], MaxWinnerNameLengthInRound(tournament.Rounds[indexer - 1]), new List<int>(newMiddles), out newMiddles);
                }

                for (int i = 0; i < currRound.Count; i++)
                {
                    schemaBuilder[i].Append(currRound[i]);
                }

            }

            StringBuilder finalSchema = new StringBuilder($"Tournament '{tournament.Name}' schema:\n\n");
            schemaBuilder.ForEach(sb => finalSchema.AppendLine(sb.ToString()));
            return finalSchema.ToString();
        }
    }
}
