using Microsoft.EntityFrameworkCore;
using TournamentManager3000.Controllers.Helpers;
using TournamentManager3000.Data;
using TournamentManager3000.Models;
using TournamentManager3000.UI;

namespace TournamentManager3000.Controllers
{
    using MenuInput = List<string>;
    public class TournamentController
    {
        private readonly TournamentContext _tournamentContext;
        private readonly TournamentCreator _tournamentCreator;
        private Tournament _currentTournament = new Tournament();
        private Dictionary<Player, (int WinsCount, int LossesCount, int MatchesPlayed)> _shadowPlayerStats = new Dictionary<Player, (int WinsCount, int LossesCount, int MatchesPlayed)>();
        public bool HasTournamentStarted { get; private set; } = false;


        public TournamentController(TournamentContext tournamentContext, TournamentCreator tournamentCreator, ConsoleProvider consoleProvider)
        {
            _tournamentContext = tournamentContext;
            _tournamentCreator = tournamentCreator;
        }


        private bool CheckOngoingTournament(bool shouldBeStarted, out string message)
        {
            if (shouldBeStarted && !HasTournamentStarted)
            {
                message = "There is no ongoing tournament. Please create one first.";
                return false;
            }
            else if (!shouldBeStarted && HasTournamentStarted)
            {
                message = "There is already an ongoing tournament. You have to exit this one first.";
                return false;
            }
            message = "";
            return true;
        }

        public string MenuName(MenuInput _) => "Tournament menu";

        public string Help(MenuInput _)
        {
            return CommonMessages.HELP_HEADER +
                CommonMessages.HELP_OPTION +
                "'create-tournament <NAME> <list of player ID/nicknames>' - creates new tournament with given players specified by their ID or nickname (separate with spaces)\n" +
                "'add-result <MATCH NUM> <WINNER ID or WINNER NICKNAME>' - sets a winner for a given match (from current tournament)\n" +
                "'show-tournament <ID>' - shows details about given tournament\n" +
                "'show-round <TOURNAMENT ID> <ROUND NUM>' - shows deatils about selected round from given tournament\n" +
                "'list-tournaments' - lists all stored tournaments\n" +
                "'delete-tournament <ID>' - deletes tournament with given ID" +
                CommonMessages.BACK_TO_MAIN;
        }

        public string Back(MenuInput _)
        {
            string message = "Exiting Tournament menu.";
            if (HasTournamentStarted)
            {
                message = $"Ongoing tournament '{_currentTournament.Name}' dropped. " + message;
            }
            HasTournamentStarted = false;
            _currentTournament = new Tournament();   // to not store entire tournament in memory for nothing
            return message;
        }

        public string DeleteTournament(MenuInput input)
        {
            string message;
            if (!CommonMethods.CheckListLength(input, 1, 1, out message)) return message;

            int tournamentId;
            if (!int.TryParse(input[0], out tournamentId)) return "First" + CommonMessages.NUM_ARG;

            Tournament tournamentToDelete;
            if (!_tournamentCreator.TryParseTournament(tournamentId, _tournamentContext, out tournamentToDelete)) return "Tournament" + CommonMessages.ID_NOT_FOUND;

            _tournamentContext.Tournaments.Remove(tournamentToDelete);
            _tournamentContext.SaveChanges();
            return $"Tournament with ID '{tournamentId}' and name '{tournamentToDelete.Name}'" + CommonMessages.SUCC_DEL;
        }

        public string ShowTournament(MenuInput input)
        {
            string message;
            if (!CommonMethods.CheckListLength(input, 1, 1, out message)) return message;

            int tournamentId;
            if (!int.TryParse(input[0], out tournamentId)) return "First" + CommonMessages.NUM_ARG;

            Tournament tournament;
            if (!_tournamentCreator.TryParseTournament(tournamentId, _tournamentContext, out tournament)) return "Tournament" + CommonMessages.ID_NOT_FOUND;

            return CommonMethods.BuildTableFromDictionary(new Dictionary<string, List<string>>
            {
                {"ID", new List<string>() { tournament.Id.ToString() } },
                {"Name", new List<string>() { tournament.Name} },
                {"Rounds", new List<string>() { tournament.Rounds.Count.ToString() } },
                {"Matches", new List<string>() { tournament.Rounds.Select(r => r.Matches.Count).Sum().ToString() } },
                {"Winner", new List<string>() { tournament.Rounds.Last().Matches.Last().Winner!.Nickname } },
            }) + "\n\n" + _tournamentCreator.TournamentToSchema(tournament, false);
        }


        public string ShowRound(MenuInput input)
        {
            string message;
            if (CommonMethods.CheckListLength(input, 2, 2, out message)) return message;

            int tournamentId;
            if (!int.TryParse(input[0], out tournamentId)) return "First" + CommonMessages.NUM_ARG;

            int roundNum;
            if (!int.TryParse(input[1], out roundNum)) return "Second" + CommonMessages.NUM_ARG;

            Tournament tournament;
            if (!_tournamentCreator.TryParseTournament(tournamentId, _tournamentContext, out tournament)) return "Tournament with given ID does not exist!";

            if (roundNum < 1 || roundNum > tournament.Rounds.Count) return $"Given round number is out of range for selected Tournament '{tournament.Name}'.";

            return RoundToString(tournament.Rounds[roundNum - 1]);
        }


        public string ListTournaments(MenuInput input)
        {
            var allStoredTournaments = _tournamentContext.Tournaments.Include(t => t.Rounds).ThenInclude(r => r.Matches).ThenInclude(m => m.Winner).ToList();
            if (allStoredTournaments.Count == 0) return "No tournaments stored yet.";

            return CommonMethods.BuildTableFromDictionary(new Dictionary<string, List<string>>
            {
                {"ID", allStoredTournaments.Select(t => t.Id.ToString()).ToList() },
                {"Name", allStoredTournaments.Select(t => t.Name).ToList() },
                {"Rounds", allStoredTournaments.Select(t => t.Rounds.Count.ToString()).ToList() },
                {"Matches", allStoredTournaments.Select(t => t.Rounds.Select(r => r.Matches.Count).Sum().ToString()).ToList() },
                {"Winner", allStoredTournaments.Select(t => PlayerToString(t.Rounds.Last().Matches.Last().Winner!)).ToList() },
            });
        }


        private string PlayerToString(Player player)
        {
            return $"ID: '{player.Id}', Nickname: '{player.Nickname}'";
        }


        private string RoundToString(Round round)
        {
            return $"Round number {round.RoundNumber}:\n\n" + CommonMethods.BuildTableFromDictionary(new Dictionary<string, List<String>>
            {
                {"Match number", Enumerable.Range(1, round.Matches.Count).Select(x => x.ToString()).ToList() },
                {"Player One", round.Matches.Select(m => PlayerToString(m.Player1)).ToList() },
                {"Player Two", round.Matches.Select(m => m.Player2 != null ? PlayerToString(m.Player2) : "Auto Advance").ToList() },
                {"Winner", round.Matches.Select(m => m.Winner != null ? PlayerToString(m.Winner) : "No winner yet").ToList() }
            });
        }


        public string Create(MenuInput input)
        {
            string message;
            if (!CheckOngoingTournament(false, out message)) return message;
            if (!CommonMethods.CheckListLength(input, 3, int.MaxValue, out message)) return message;

            List<string> playerIdsOrNicknames = input.Skip(1).ToList();
            List<Player> players;
            if (!_tournamentCreator.TryLoadPlayers(playerIdsOrNicknames, _tournamentContext, out players)) return $"Given argument '{playerIdsOrNicknames[players.Count]}' is not valid ID nor nickname.";
            if (_tournamentCreator.ContainsDuplicate(players, out var duplicatePlayer)) return $"More than one ID or nickname refer to the same player (ID: {duplicatePlayer.Id}, Nickname: {duplicatePlayer.Nickname}).";

            string tournamentName = input[0];

            _shadowPlayerStats = players.ToDictionary(p => p, p => (0, 0, 0));
            _currentTournament = _tournamentCreator.CreateTournamentAndFirstRound(players, tournamentName);

            HasTournamentStarted = true;

            return $"Tournament '{tournamentName}' started. You can add match result by 'add-result' command. Type 'help' for more information.\n" + RoundToString(_currentTournament.Rounds.First());
        }


        private string FinishTournament(Player winner)
        {

            using (var transaction = _tournamentContext.Database.BeginTransaction())
            {
                try
                {
                    _tournamentContext.Tournaments.Add(_currentTournament);

                    foreach (var player in _shadowPlayerStats.Keys)
                    {
                        player.Wins += _shadowPlayerStats[player].WinsCount;
                        player.Losses += _shadowPlayerStats[player].LossesCount;
                        player.MatchesPlayed += _shadowPlayerStats[player].MatchesPlayed;
                        player.TournamentWins += player == winner ? 1 : 0;
                    }

                    _tournamentContext.Players.UpdateRange(_shadowPlayerStats.Keys.ToList());

                    _tournamentContext.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            HasTournamentStarted = false;
            return $"\nTournament has ended and has been saved with ID '{_currentTournament.Id}'.\nAbsolute winner is {winner.Nickname}. Congratulations!\n";
        }


        public string SetMatchWinner(MenuInput input)
        {
            if (!CheckOngoingTournament(true, out var tourErrMessage)) return tourErrMessage;
            if (!CommonMethods.CheckListLength(input, 2, 2, out var listErrMessage)) return listErrMessage;
            if (!int.TryParse(input[0], out int matchNumber)) return "First argument must be a number!";

            Round currentRound = _currentTournament.Rounds.Last();
            if (matchNumber < 1 || matchNumber > currentRound.Matches.Count) return "Invalid match number";
            if (!CommonMethods.TryParsePlayer(input[1], _tournamentContext, out Player player)) return "Player does not exist";

            var match = currentRound.Matches[matchNumber - 1];
            if (!(match.Player1 == player) && !(match.Player2 == player)) return "Player is not in the selected match";
            if (match.Winner != null) return "Match already has a winner";

            match.Winner = player;

            var winnerShadowStats = _shadowPlayerStats[player];
            _shadowPlayerStats[player] = (winnerShadowStats.WinsCount + 1, winnerShadowStats.LossesCount, winnerShadowStats.MatchesPlayed + 1);

            Player loserPlayer = match.Player1 == player ? match.Player2! : match.Player1;
            var loserShadowStats = _shadowPlayerStats[loserPlayer];
            _shadowPlayerStats[loserPlayer] = (loserShadowStats.WinsCount, loserShadowStats.LossesCount + 1, loserShadowStats.MatchesPlayed + 1);


            string outputString = RoundToString(currentRound);
            if (currentRound.Matches.Count == 1) return outputString + FinishTournament(match.Winner);

            if (!currentRound.Matches.Any(m => m.Winner == null))
            {
                Round newRound = _tournamentCreator.StartNewRound(_currentTournament);

                return outputString + $"\n\nRound {currentRound.RoundNumber} ended. Starting new round:\n" + RoundToString(newRound);
            }

            return outputString;
        }
    }
}
