using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TournamentManager3000.Controllers.Helpers;
using TournamentManager3000.Data;
using TournamentManager3000.Models;

namespace TournamentManager3000.Controllers
{
    using MenuInput = List<string>;

    public class ImportExportController
    {
        private readonly TournamentContext _tournamentContext;
        private readonly TournamentCreator _tournamentCreator;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions() { WriteIndented = true };
        public ImportExportController(TournamentCreator tournamentCreator, TournamentContext tournamentContext)
        {
            _tournamentContext = tournamentContext;
            _tournamentCreator = tournamentCreator;
        }

        public string MenuName(MenuInput input) => "Import/Export";

        public string Help(MenuInput input)
        {
            return CommonMessages.HELP_HEADER +
                "FILEPATH cannot contain any spaces!\n" +
                CommonMessages.HELP_OPTION +
                "'import-players <FILEPATH>' - imports players from given file in JSON format\n" +
                "'export-players <FILEPATH>' - exports stored players to given directory or file in JSON format\n" +
                "'export-tournament <FILEPATH> <TOURNAMENT ID>' - exports filled schema to given directory TXT file\n" +
                "'export-empty-tournament <FILEPATH> <list of PLAYER IDs or NICKNAMEs>' - exports empty schema to given directory or TXT file\n";
        }

        public string Exit(MenuInput _) => "Leaving Import/Export menu";

        public string ImportPlayers(MenuInput input)
        {
            var message = "";
            if (!CommonMethods.CheckListLength(input, 1, 1, out message)) return message;

            string path = String.Join("", input);
            List<Player>? loadedPlayers;

            if (!path.EndsWith(".json")) return "File must be in JSON format!";

            try
            {
                FileStream inputFile = new FileStream(path, FileMode.Open);
                loadedPlayers = JsonSerializer.Deserialize<List<Player>>(inputFile);
            } catch (Exception ex)
            {
                return "Error occured while importing players: " + ex.Message;
            }

            if (loadedPlayers == null || loadedPlayers.Count == 0)
            {
                return "No players found in given file.";
            }

            var alreadyStoredPlayers = _tournamentContext.Players.ToList();
            string duplicateNickname = "";
            if (_tournamentCreator.ContainsDuplicateByNickname(alreadyStoredPlayers.Concat(loadedPlayers).ToList(), out duplicateNickname)) return $"At least one of the imported players has duplicit nickname or nickname is already in use ('{duplicateNickname}')!";
            _tournamentContext.Players.AddRange(loadedPlayers);
            _tournamentContext.SaveChanges();
            return $"{loadedPlayers.Count} player{(loadedPlayers.Count > 1 ? "s" : "")} successfuly imported.";
        }

        public string ExportPlayers(MenuInput input)
        { 
            var message = "";
            if (!CommonMethods.CheckListLength(input, 1, 1, out message)) return message;

            var storedPlayers = _tournamentContext.Players.ToList();
            string path = String.Join("", input);

            if (Directory.Exists(path))
            {
                path += "\\players.json";
            } else if (!path.EndsWith(".json"))
            {
                path += ".json";
            }

            try
            {
                FileStream outputFile = new FileStream(input[0], FileMode.Create);
                JsonSerializer.Serialize(outputFile, storedPlayers, _jsonSerializerOptions);
                outputFile.Dispose();
            } catch (Exception ex)
            {
                return "Error occured while export players: " + ex.Message;
            }

            return $"Players were successfuly exported to '{path}'.";
        }


        private string FormatName(string name, int maxNameLength)
        {
            return $"[ {name} ]" + new string('-', maxNameLength - name.Length);
        }


        private StringBuilder RoundToSchema(Round? round, Round? prevRound, int maxNameLength, List<int> prevMiddles)
        {
            StringBuilder sb = new StringBuilder();
            int horizontalLineLength = 5;
            int writeLength = maxNameLength + 4 + horizontalLineLength; // 4 for brackets and spaces around name
            int totalLength = writeLength + horizontalLineLength;
            string endFillament = new string(' ', totalLength - writeLength);
            string horizontalLine = new string('-', horizontalLineLength - 1);
            string regHorizontalLine = horizontalLine + "+" + endFillament;
            string endingHorizontalLine = new string(' ', writeLength - 1) + "+" + horizontalLine;

            string verticalLine = new string(' ', writeLength - 1) + "|" + endFillament;
            string emptyLine = new string(' ', totalLength);

            List<int> currNameIndexes = new List<int>();
            string? fillement = null;
            if (round == null) fillement = new string(' ', maxNameLength);

            for (int i = 0; i < prevMiddles[0]; i++) sb.AppendLine(emptyLine);
            if (prevMiddles.Count == 1)
            {
                sb.AppendLine($"{{ {(fillement == null ? prevRound!.Matches.First().Winner!.Nickname : fillement)} }}");
                for (int i = 0; i < prevMiddles[0]; i++) sb.AppendLine(emptyLine);
                return sb;
            }

            for (int i = 0; i < prevMiddles.Count; i += (prevRound == null ? 1 : 2))
            {
                if (fillement != null)
                {
                    sb.AppendLine(FormatName(fillement, maxNameLength) + regHorizontalLine);
                } else if (prevRound == null)
                {
                    sb.AppendLine(FormatName(round!.Matches[i].Player1.Nickname, maxNameLength) + regHorizontalLine);
                } else
                {
                    sb.AppendLine(FormatName(prevRound!.Matches[i].Winner!.Nickname, maxNameLength) + regHorizontalLine);
                }

                var newGap = prevMiddles[i] - prevMiddles[i + 1];
                var newMiddle = newGap / 2;
                currNameIndexes.Add(prevMiddles[i] + newMiddle);
                var verticalLineCount = newMiddle - 1;

                for (int j = 0; j < verticalLineCount; j++) sb.AppendLine(verticalLine);
                sb.AppendLine(endingHorizontalLine);
                for (int j = 0; j < verticalLineCount; j++) sb.AppendLine(verticalLine);

                /*
                var prevFirst = prevNamesIndexes[i];
                var prevSecond = prevNamesIndexes[i + 1];
                var newFirst = (prevSecond - prevFirst) / 2;

                var prevThird = prevNamesIndexes[i + 2];
                var prevFourth = prevNamesIndexes[i + 3];
                var newSecond = (prevThird - prevFourth) / 2;

                var newGap = (newFirst - prevSecond);
                var newMiddle = newGap / 2;
                var verticalLineCount = newMiddle - 1;
                */



                if (fillement != null)
                {
                    sb.AppendLine(FormatName(fillement, maxNameLength) + regHorizontalLine);
                }
                else if (prevRound == null)
                {
                    sb.AppendLine(FormatName(round!.Matches[i].Player2 != null ? round!.Matches[i / 2].Player2!.Nickname : "Auto Advance", maxNameLength) + regHorizontalLine);
                }
                else
                {
                    sb.AppendLine(FormatName(round!.Matches[i + 1].Winner!.Nickname, maxNameLength) + regHorizontalLine);
                }

                if (prevMiddles.Count > i + 2)
                {
                    for (int j = 0; j < prevMiddles[0]; j++) sb.AppendLine(emptyLine);
                }
            }
            return sb;
        }


        private string TournamentToSchema(Tournament tournament)
        {
            List<int> initMiddles = new List<int>();
            int counter = 0;
            for (int i = 0; i < tournament.Rounds.First().Matches.Count * 2; i++)
            {
                initMiddles.Add(counter);
                counter += (i % 2 == 0 ? 4 : 2);
            }
            round.Matches.Select(m => Math.Max(m.Player1.Nickname.Length, m.Player2 != null ? m.Player2.Nickname.Length : 0)).Max();
        }


        public string ExportTournamentSchema(MenuInput input)
        {
            var message = "";
            if (!CommonMethods.CheckListLength(input, 1, int.MaxValue, out message)) return message;

            var path = String.Join("", input);

            if (Directory.Exists(path))
            {
                path += "emptyTournamentSchema.txt";
            } else if (!path.EndsWith(".txt"))
            {
                path += ".txt";
            }

            int tournamentId = 0;
            if (!int.TryParse(input[1], out tournamentId)) return "First" + CommonMessages.NUM_ARG;

            Tournament tournament = new Tournament();
            if (!_tournamentCreator.TryParseTournament(tournamentId, _tournamentContext, out tournament)) return "Tournament not found along stored tournaments.";

            return TournamentToSchema(tournament);
        }
    }
}
