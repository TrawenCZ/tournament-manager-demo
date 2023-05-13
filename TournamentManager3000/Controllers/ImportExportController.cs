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


        private string RoundToSchema(Round? round, Round? prevRound, int maxNameLength, List<int> previousIndents)
        {
            StringBuilder sb = new StringBuilder();
            string horizontalLine = new string('-', 4);
            string verticalLine = "|";
            List<int> currIndents = new List<int>();
            string? fillement = null;
            if (round == null) fillement = new string(' ', maxNameLength);

            for (int i = 0; i < previousIndents.Count; i++)
            {
                if (fillement != null)
                {

                }
                if (match.Player2 == null)
                {
                    sb.AppendLine(match.Player1.Nickname.PadRight(maxNameLength));
                }
            }
        }


        private string TournamentToSchema(Tournament tournament)
        {
            Dictionary<Round, List<int>> roundIndents = new Dictionary<Round, List<int>>();
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
