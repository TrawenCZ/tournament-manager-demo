﻿using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TournamentManager3000.Controllers.Helpers;
using TournamentManager3000.Models;
using TournamentManager3000.UI;

namespace TournamentManager3000.Controllers
{
    using MenuInput = List<string>;
    public class TournamentController
    {
        private readonly TournamentContext _tournamentContext;
        private readonly ConsoleProvider _consoleProvider;
        private Tournament _currentTournament = new Tournament();
        private List<Player> _remainingPlayers = new List<Player>();
        private Player _dummyPlayer = new Player() { Id = -1, Nickname = "Dummy" };
        private Dictionary<Player, (int WinsCount, int LossesCount, int MatchesPlayed)> _shadowPlayerStats = new Dictionary<Player, (int WinsCount, int LossesCount, int MatchesPlayed)>();
        public bool HasTournamentStarted { get; private set; } = false;


        public TournamentController(TournamentContext tournamentContext, ConsoleProvider consoleProvider)
        {
            _tournamentContext = tournamentContext;
            _consoleProvider = consoleProvider;
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
            else
            {
                message = "";
                return true;
            }
        }

        public string MenuName(MenuInput _) => "Tournament";

        public string Help(MenuInput _)
        {
            return CommonMessages.HELP_HEADER +
                CommonMessages.HELP_OPTION +
                "'exit' - exits Tournament menu\n" +
                "'create-tournament <NAME> <DESCRIPTION>?' - creates new tournament.\n" +
                "'add-result <MATCH NUM> <WINNER ID or WINNER NICKNAME>' - sets a winner for a given match (from current tournament)\n" +
                "'show-tournament <ID>' - shows details about given tournament\n" +
                "'show-round <TOURNAMENT ID> <ROUND NUM>' - shows deatils about selected round from given tournament\n" +
                "'list-tournaments' - lists all stored tournaments\n";
        }

        public string Exit(MenuInput _)
        {
            string message = "Exiting Tournament menu.";
            if (HasTournamentStarted)
            {
                message = $"Ongoing tournament '{_currentTournament.Name}' dropped. " + message;
            }
            return message;
        }

        public string ShowTournament(MenuInput input)
        {
            var message = "";
            if (!CommonMethods.CheckListLength(input, 1, 1, out message)) return message;

            int tournamentId = 0;
            if (!int.TryParse(input[0], out tournamentId)) return "First argument must be a number.";

            Tournament tournament = new Tournament();
            if (!TryParseTournament(tournamentId, out tournament)) return "Tournament with given ID does not exist!";

            return CommonMethods.BuildTableFromDictionary(new Dictionary<string, List<string>> 
            {
                {"ID", new List<string>() { tournament.Id.ToString() } },
                {"Name", new List<string>() { tournament.Name} },
                {"Rounds", new List<string>() { tournament.Rounds.Count.ToString() } },
                {"Matches", new List<string>() { tournament.Rounds.Select(r => r.Matches.Count).Sum().ToString() } },
                {"Winner", new List<string>() { tournament.Rounds.Last().Matches.Last().Winner!.Nickname } },
            });
        }


        public string ShowRound(MenuInput input)
        {
            var message = "";
            if (CommonMethods.CheckListLength(input, 2, 2, out message)) return message;

            var tournamentId = 0;
            if (!int.TryParse(input[0], out tournamentId)) return "First argument must be a number.";

            var roundId = 0;
            if (!int.TryParse(input[1], out roundId)) return "Seconds argument must be a number.";

        }

        private bool TryParseTournament(int tournamentId, out Tournament tournament)
        {
            tournament = new Tournament();
            var tournamentLoaded = _tournamentContext.Tournaments.FirstOrDefault(t => t.Id == tournamentId);
            if (tournamentLoaded != null)
            {
                tournament = tournamentLoaded;
                return true;
            } else return false;
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


        private bool TryParsePlayer(string idOrNickname, out Player player)
        {
            Player? playerOrDefault = null;
            player = new Player();
            if (int.TryParse(idOrNickname, out int id))
            {
                playerOrDefault = _tournamentContext.Players.FirstOrDefault(p => p.Id == id && !p.IsDeleted);
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
                playerOrDefault = _tournamentContext.Players.FirstOrDefault(p => p.Nickname == idOrNickname && !p.IsDeleted);
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


        private bool TryLoadPlayers(List<string> idOrNicknameList, out List<Player> players)
        {
            players = new List<Player>();
            foreach (var idOrNickname in idOrNicknameList)
            {
                if (TryParsePlayer(idOrNickname, out Player player))
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


        private bool ContainsDuplicate(List<Player> players, out Player duplicate)
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


        private void RandomInsertDummies()
        {
            int numOfPlayers = _remainingPlayers.Count;

            // closest power of 2 higher than numOfPlayers
            int numOfDummyPlayers = (int)Math.Pow(2, Math.Ceiling(Math.Log(numOfPlayers, 2))) - numOfPlayers;

            Shuffle(_remainingPlayers);
            for (int i = 0; i < numOfDummyPlayers * 2; i += 2)
            {
                _remainingPlayers.Insert(i + 1, _dummyPlayer);
            }
        }


        private string PlayerToString(Player player, bool autoAdvancePostfix = false)
        {
            string spaces = "      ";
            return $"{spaces}Id: {player.Id}\n{spaces}Nickname: {player.Nickname}\n" + (autoAdvancePostfix ? $"{spaces}This player auto advances to next round.\n" : "");
        }


        private string RoundToString(Round round)
        {

            StringBuilder sb = new StringBuilder();

            // Round number
            sb.AppendLine($"Round {round.RoundNumber}:");

            // Matches
            for (int i = 0; i < round.Matches.Count; i++)
            {
                Models.Match match = round.Matches[i];
                sb.AppendLine($"  Match number '{i + 1}':");
                sb.AppendLine($"    Player 1:");
                sb.AppendLine(PlayerToString(match.Player1, match.Player2 == null));
                if (match.Player2 != null)
                {
                    sb.AppendLine($"    Player 2:");
                    sb.Append(PlayerToString(match.Player2));
                }
                if (match.Winner != null)
                {
                    sb.AppendLine($"    Winner:");
                    sb.AppendLine(PlayerToString(match.Winner, false));
                }
                sb.AppendLine();
            }

            return sb.ToString();
            /*
            StringBuilder sbForMatchNumber = new StringBuilder();
            StringBuilder sbForIds = new StringBuilder();
            StringBuilder sbForNicknames = new StringBuilder();
            sbForIds.AppendLine($"Round {round.RoundNumber}:");

            foreach (var match in round.Matches)
            {
                string dummyOrRealNickname = match.Player1 == null ? "Auto Advance" : match.Player1.Nickname;
                string dummyIdOrRealId = match.Player1 == null ? "" : match.Player1.Id.ToString();
                List<string> paddedPlayer1 = CommonMethods.StringsToPadded(new List<string>() { dummyIdOrRealId, dummyOrRealNickname});
                List<string> paddedPlayer2 = CommonMethods.StringsToPadded(new List<string>() { match.Player2Id.ToString(), match.Player2.Nickname });
                sbForIds.AppendLine($"   {paddedPlayer1[0]} vs {paddedPlayer2[0]}");
                sbForNicknames.AppendLine($"   {paddedPlayer1[1]} vs {paddedPlayer2[1]}");
            }
            return sb.ToString();*/
        }


        public string Create(MenuInput input)
        {
            var message = "";
            if (!CheckOngoingTournament(false, out message)) return message;
            if (!CommonMethods.CheckListLength(input, 1, 2, out message)) return message;

            List<string> consoleInput = _consoleProvider.ReadAndSplitLine("Please write the nicknames or ids of the players you want to add to the tournament. Separate them with a space. (e.g. 1 XxProPlayerxX 94 jozkobeznozko):");
            if (!CommonMethods.CheckListLength(consoleInput, 2, int.MaxValue, out message)) return message;

            List<Player> players = new List<Player>();
            if (!TryLoadPlayers(consoleInput, out players)) return $"Given argument at position {players.Count} ('{players.Last()}') is not valid ID nor nickname.";
            if (ContainsDuplicate(players, out var duplicatePlayer)) return $"More than one ID or nickname refer to the same player (ID: {duplicatePlayer.Id}, Nickname: {duplicatePlayer.Nickname}).";

            string tournamentName = input[0];
            string? description = input.Count == 2 ? input[1] : null;

            _shadowPlayerStats = players.ToDictionary(p => p, p => (0, 0, 0));
            _remainingPlayers = players;
            RandomInsertDummies();
            _currentTournament = new Tournament() { Name = tournamentName, Description = description };
            HasTournamentStarted = true;

            Round newRound = StartNewRound();
            return $"Tournament '{tournamentName}' started. You can add match result by 'add-result' command. Type 'help' for more information.\n" + RoundToString(newRound);
        }


        private string FinishTournament(string winnerNickname)
        {
            using (var transaction = _tournamentContext.Database.BeginTransaction())
            {
                try 
                {
                    foreach (Round round in _currentTournament.Rounds)
                    {
                        _tournamentContext.Matches.AddRange(round.Matches);
                        _tournamentContext.SaveChanges();
                        _tournamentContext.Rounds.Add(round);
                        _tournamentContext.SaveChanges();
                    }

                    _tournamentContext.Tournaments.Add(_currentTournament);

                    foreach (var player in _shadowPlayerStats.Keys)
                    {
                        player.Wins += _shadowPlayerStats[player].WinsCount;
                        player.Losses += _shadowPlayerStats[player].LossesCount;
                        player.MatchesPLayed += _shadowPlayerStats[player].MatchesPlayed;
                    }

                    _tournamentContext.Players.UpdateRange(_shadowPlayerStats.Keys);

                    _tournamentContext.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return $"\nTournament has ended and has been saved with ID '{_currentTournament.Id}'.\nAbsolute winner is {winnerNickname}. Congratulations!\n";
        }


        private Round StartNewRound()
        {
            var newMatches = new List<Models.Match>();
            int numOfPlayers = _remainingPlayers.Count;
            var newMatch = new Models.Match();
            if (_currentTournament.Rounds.Count == 0)
            {
                for (int i = 0; i < numOfPlayers * 2; i += 2)
                {
                    newMatch = new Models.Match() { Player1 = _remainingPlayers[i], Player2 = (_remainingPlayers[i + 1] == _dummyPlayer ? null : _remainingPlayers[i + 1]) };
                    newMatches.Add(newMatch);
                }
            } else
            {
                List<Models.Match> currRoundMatches = _currentTournament.Rounds.Last().Matches;
                for (int i = 0; i < currRoundMatches.Count * 2; i += 2)
                {
                    newMatch = new Models.Match() { Player1 = currRoundMatches[i].Winner!, Player2 = currRoundMatches[i + 1].Winner! };
                    newMatches.Add(newMatch);
                    _remainingPlayers.Remove(currRoundMatches[i].Player2 == currRoundMatches[i].Winner ? currRoundMatches[i].Player2! : currRoundMatches[i].Player1);
                    _remainingPlayers.Remove(currRoundMatches[i].Player2 == currRoundMatches[i].Winner ? currRoundMatches[i].Player2! : currRoundMatches[i].Player1);
                }
            }
            var round = new Round() { Matches = newMatches, RoundNumber = _currentTournament.Rounds.Count + 1 };
            _currentTournament.Rounds.Add(round);
            return round;
        }


        public string SetMatchWinner(MenuInput input)
        {
            if (!CheckOngoingTournament(true, out var tourErrMessage)) return tourErrMessage;
            if (!CommonMethods.CheckListLength(input, 2, 2, out var listErrMessage)) return listErrMessage;
            if (!int.TryParse(input[0], out int matchNumber)) return "First argument must be a number!";

            Round currentRound = _currentTournament.Rounds.Last();
            if (matchNumber < 0 || matchNumber > currentRound.Matches.Count) return "Invalid match number";
            if (!TryParsePlayer(input[1], out Player player)) return "Player does not exist";

            var match = currentRound.Matches[matchNumber - 1];
            if (!(match.Player1 == player) && !(match.Player2 == player)) return "Player is not in the selected match";
            if (match.Winner != null) return "Match already has a winner";

            match.Winner = player;

            var winnerShadowStats = _shadowPlayerStats[player];
            _shadowPlayerStats[player] = (winnerShadowStats.WinsCount + 1, winnerShadowStats.LossesCount, winnerShadowStats.MatchesPlayed + 1);

            Player loserPlayer = match.Player2 == player ? match.Player2 : match.Player1;
            var loserShadowStats = _shadowPlayerStats[loserPlayer];
            _shadowPlayerStats[loserPlayer] = (loserShadowStats.WinsCount, loserShadowStats.LossesCount + 1, loserShadowStats.MatchesPlayed + 1);


            string outputString = RoundToString(currentRound);
            if (currentRound.Matches.Count == 1) return outputString + FinishTournament(match.Winner.Nickname);

            if (!currentRound.Matches.Any(m => m.Winner == null))
            {
                Round newRound = StartNewRound();

                return outputString + $"Round {currentRound.RoundNumber} ended. Starting new round:\n" + RoundToString(newRound);
            }

            return outputString;
        }
    }
}
