using System.Text.Json;
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

        public string Back(MenuInput _) => "Leaving Import/Export menu";

        public string ImportPlayers(MenuInput input)
        {
            string message;
            if (!CommonMethods.CheckListLength(input, 1, 1, out message)) return message;

            string path = input[0];
            List<Player>? loadedPlayers;

            if (Path.GetExtension(path) != ".json") return "File must be in JSON format!";

            try
            {
                FileStream inputFile = new FileStream(path, FileMode.Open);
                loadedPlayers = JsonSerializer.Deserialize<List<Player>>(inputFile);
            }
            catch (Exception ex)
            {
                return "Error occured while importing players:\n" + ex.Message;
            }

            if (loadedPlayers == null || loadedPlayers.Count == 0)
            {
                return "No players found in given file.";
            }

            var alreadyStoredPlayers = _tournamentContext.Players.Where(p => !p.IsDeleted).ToList();
            string duplicateNickname;
            if (_tournamentCreator.PlayersContainDuplicatByNickname(alreadyStoredPlayers.Concat(loadedPlayers).ToList(), out duplicateNickname)) return $"At least one of the imported players has duplicit nickname or nickname is already in use ('{duplicateNickname}')!";

            _tournamentContext.Players.AddRange(loadedPlayers);
            _tournamentContext.SaveChanges();
            return $"{loadedPlayers.Count} player{(loadedPlayers.Count > 1 ? "s" : "")} successfuly imported:\n\n" + CommonMethods.ListPlayers(loadedPlayers);
        }

        public string ExportPlayers(MenuInput input)
        {
            string message;
            if (!CommonMethods.CheckListLength(input, 1, 1, out message)) return message;

            var storedPlayers = _tournamentContext.Players.Where(p => !p.IsDeleted).ToList();
            string path = input[0];

            if (Directory.Exists(path))
            {
                path = Path.Combine(path, "players.json");
            }
            else if (Path.GetExtension(path) != ".json")
            {
                path = Path.ChangeExtension(path, ".json");
            }

            try
            {
                FileStream outputFile = new FileStream(path, FileMode.Create);
                JsonSerializer.Serialize(outputFile, storedPlayers, _jsonSerializerOptions);
                outputFile.Dispose();
            }
            catch (Exception ex)
            {
                return "Error occured while export players:\n" + ex.Message;
            }

            return $"'{storedPlayers.Count}' players were successfuly exported to '{path}'.";
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
            string message;
            if (!CommonMethods.CheckListLength(input, (isEmpty ? 3 : 2), (isEmpty ? int.MaxValue : 2), out message)) return message;

            Tournament tournament;
            if (!isEmpty)
            {
                int tournamentId;
                if (!int.TryParse(input[1], out tournamentId)) return "First" + CommonMessages.NUM_ARG;

                tournament = new Tournament();
                if (!_tournamentCreator.TryParseTournament(tournamentId, _tournamentContext, out tournament)) return "Tournament not found along stored tournaments.";
            }
            else
            {
                List<string> playerIdsOrNicknames = input.Skip(2).ToList();
                List<Player> players = new List<Player>();
                if (!_tournamentCreator.TryLoadPlayers(playerIdsOrNicknames, _tournamentContext, out players)) return $"Player ID/nickname '{playerIdsOrNicknames[players.Count]}' was not found.";

                if (_tournamentCreator.PlayersContainDuplicate(players, out var player)) return $"Player '{player.Nickname}' was found multiple times.";

                tournament = _tournamentCreator.CreateTournamentAndFirstRound(players, input[1]);
            }

            string schema = _tournamentCreator.TournamentToSchema(tournament, isEmpty);

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
            }
            catch (Exception e)
            {
                return "Error occured while saving schema to file:\n" + e.Message;
            }

            return "Successfuly created and exported schema for given tournament:\n\n" + schema;
        }
    }
}
