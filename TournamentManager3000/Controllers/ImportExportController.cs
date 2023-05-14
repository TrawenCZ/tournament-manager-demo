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
        public ImportExportController(TournamentContext tournamentContext, TournamentCreator tournamentCreator)
        {
            _tournamentContext = tournamentContext;
            _tournamentCreator = tournamentCreator;
        }

        public string MenuName(MenuInput input) => "Import/Export menu";

        public string Help(MenuInput input)
        {
            return CommonMessages.HELP_HEADER +
                "FILEPATH cannot contain any spaces!\n" +
                CommonMessages.HELP_OPTION +
                "'import-players <FILEPATH>' - imports players from given file in JSON format\n" +
                "'export-players <FILEPATH>' - exports stored players to given directory or file in JSON format\n" +
                "'export-tournament <FILEPATH> <TOURNAMENT ID>' - exports filled schema to given directory TXT file\n" +
                "'export-empty-tournament <FILEPATH> <TOURNAMENT NAME> <list of PLAYER IDs or NICKNAMEs>' - exports empty schema to given directory or TXT file\n" +
                CommonMessages.BACK_TO_MAIN;
        }

        public string Back(MenuInput _) => "Leaving Import/Export menu";

        public string ImportPlayers(MenuInput input)
        {
            var message = "";
            if (!CommonMethods.CheckListLength(input, 1, 1, out message)) return message;

            string path = input[0];
            List<Player>? loadedPlayers;

            if (Path.GetExtension(path) != ".json") return "File must be in JSON format!";

            try
            {
                FileStream inputFile = new FileStream(path, FileMode.Open);
                loadedPlayers = JsonSerializer.Deserialize<List<Player>>(inputFile);
            } catch (Exception ex)
            {
                return "Error occured while importing players:\n" + ex.Message;
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
            return $"{loadedPlayers.Count} player{(loadedPlayers.Count > 1 ? "s" : "")} successfuly imported:\n\n" + CommonMethods.ListPlayers(loadedPlayers);
        }

        public string ExportPlayers(MenuInput input)
        { 
            var message = "";
            if (!CommonMethods.CheckListLength(input, 1, 1, out message)) return message;

            var storedPlayers = _tournamentContext.Players.ToList();
            string path = input[0];

            if (Directory.Exists(path))
            {
                path = Path.Combine(path, "players.json");
            } else if (Path.GetExtension(path) != ".json")
            {
                path = Path.ChangeExtension(path, ".json");
            }

            try
            {
                FileStream outputFile = new FileStream(path, FileMode.Create);
                JsonSerializer.Serialize(outputFile, storedPlayers, _jsonSerializerOptions);
                outputFile.Dispose();
            } catch (Exception ex)
            {
                return "Error occured while export players:\n" + ex.Message;
            }

            return $"Players were successfuly exported to '{path}'.";
        }


        private string FormatName(string name, int maxNameLength)
        {
            return $"[ {name} ]" + new string('-', maxNameLength - name.Length);
        }


        private List<string> RoundToSchema(Round? round, Round? prevRound, int maxNameLength, List<int> prevMiddles, out List<int> newMiddles)
        {
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
                for (int i = 0; i < prevMiddles[0]; i++) output.Add(string.Empty);
                return output;
            }

            for (int i = 0; i < prevMiddles.Count; i += 2)
            {
                if (i > 0) for (int j = 0; j < prevMiddles[i] - prevMiddles[i - 1] - 1; j++) output.Add(emptyLine);

                if (fillement != null)
                {
                    output.Add(FormatName(fillement, maxNameLength) + regHorizontalLine);
                } else if (prevRound == null)
                {
                    output.Add(FormatName(round!.Matches[i / 2].Player1.Nickname, maxNameLength) + regHorizontalLine);
                } else
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
                    output.Add(FormatName(round!.Matches[i + 1].Winner!.Nickname, maxNameLength) + regHorizontalLine);
                }
            }

            for (int j = 0; j < prevMiddles[0]; j++) output.Add(string.Empty);
            return output;
        }


        private int MaxNameLengthInRound(Round round)
        {
            return round.Matches.Select(m => Math.Max(m.Player1.Nickname.Length, m.Player2 != null ? m.Player2.Nickname.Length : "Auto Advance".Length)).Max();
        }


        private string TournamentToSchema(Tournament tournament, bool isEmpty)
        {
            List<int> newMiddles = new List<int>();
            int counter = 0;
            for (int i = 0; i < tournament.Rounds.First().Matches.Count * 2; i++)
            {
                newMiddles.Add(counter);
                counter += (i % 2 == 0 ? 4 : 2);
            }
            int maxNameLength = MaxNameLengthInRound(tournament.Rounds.First());
            List<StringBuilder> schemaBuilder = RoundToSchema(tournament.Rounds.First(), null, maxNameLength, new List<int>(newMiddles), out newMiddles).Select(s => new StringBuilder(s)).ToList();

            int indexer = 1;
            List<string> currRound = new List<string>();
            while (newMiddles.Count != 0)
            {
                if (isEmpty)
                {
                    currRound = RoundToSchema(null, null, maxNameLength, new List<int>(newMiddles), out newMiddles);
                } else if (tournament.Rounds.Count != 1 && tournament.Rounds.Count - 1 != indexer)
                {
                    currRound = RoundToSchema(tournament.Rounds[indexer],
                        tournament.Rounds[indexer - 1],
                        MaxNameLengthInRound(tournament.Rounds[indexer++]),
                        new List<int>(newMiddles),
                        out newMiddles);
                } else
                {
                    currRound = RoundToSchema(null, tournament.Rounds[indexer - 1], maxNameLength, new List<int>(newMiddles), out newMiddles);
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


        public string ExportEmptyTournamentSchema(MenuInput input)
        {
            return ExportTournamentSchema(input, true);
        }

        public string ExportTournamentSchema(MenuInput input)
        {
            return ExportTournamentSchema(input, false);
        }


        private string ExportTournamentSchema(MenuInput input, bool isEmpty)
        {
            var message = "";
            if (!CommonMethods.CheckListLength(input, 1, (isEmpty ? int.MaxValue : 2), out message)) return message;

            Tournament tournament;
            if (!isEmpty)
            {
                int tournamentId = 0;
                if (!int.TryParse(input[1], out tournamentId)) return "First" + CommonMessages.NUM_ARG;

                tournament = new Tournament();
                if (!_tournamentCreator.TryParseTournament(tournamentId, _tournamentContext, out tournament)) return "Tournament not found along stored tournaments.";
            } else
            {
                List<string> playerIdsOrNicknames = input.Skip(2).ToList();
                List<Player> players = new List<Player>();
                if (!_tournamentCreator.TryLoadPlayers(playerIdsOrNicknames, _tournamentContext, out players)) return $"Player ID/nickname '{playerIdsOrNicknames[players.Count]}' was not found.";

                if (_tournamentCreator.ContainsDuplicate(players, out var player)) return $"Player '{player.Nickname}' was found multiple times.";

                tournament = _tournamentCreator.CreateTournamentAndFirstRound(players, input[1]);
            }

            string schema = TournamentToSchema(tournament, isEmpty);

            var path = String.Join("", input[0]);

            if (Directory.Exists(path))
            {
                path += "tournamentSchema.txt";
            }
            else if (!path.EndsWith(".txt"))
            {
                path += ".txt";
            }

            try
            {
                FileStream fileStream = new FileStream(path, FileMode.Create);
                StreamWriter streamWriter = new StreamWriter(fileStream);
                streamWriter.Write(schema);
                streamWriter.Close();
                fileStream.Close();
            } catch (Exception e)
            {
                return "Error occured while saving schema to file:\n" + e.Message;
            }

            return "Successfuly created and exported schema for given tournament:\n\n" + schema;
        }
    }
}
