using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentManager3000.Models;

namespace TournamentManager3000.Controllers
{
    public class TournamentController
    {
        private readonly TournamentContext _tournamentContext;
        private Tournament _currentTournament = new Tournament();
        private List<Player> _remainingPlayers = new List<Player>();
        private Player _dummyPlayer = new Player() { Id = -1, Nickname = "Dummy" };
        private Dictionary<int, (int WinsCount, int LossesCount)> _shadowPlayerWinsAndLosses = new Dictionary<int, (int WinsCount, int LossesCount)>();

        public bool IsTournamentStarted { get; private set; } = false;

        public TournamentController(TournamentContext tournamentContext)
        {
            _tournamentContext = tournamentContext;
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

        public bool TryLoadPlayers(List<string> idOrNicknameList, out List<Player> players)
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


        public bool ContainsDuplicate(List<Player> players, Player? duplicate)
        {
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
                _remainingPlayers.Insert(i, _dummyPlayer);
            }
        }


        public void Start(List<Player> players, string tournamentName, string? description)
        {
            _shadowPlayerWinsAndLosses = players.ToDictionary(p => p.Id, p => (0, 0));
            RandomInsertDummies();
            _currentTournament = new Tournament() { Name = tournamentName, Description = description };
            IsTournamentStarted = true;
        }


        public Round StartNewRound()
        {
            var matches = new List<Match>();
            int numOfPlayers = _remainingPlayers.Count;
            for (int i = 0; i < numOfPlayers; i += 2)
            {
                var match = new Match() { Player1 = (_remainingPlayers[i] == _dummyPlayer ? null : _remainingPlayers[i]), Player2 = _remainingPlayers[i + 1] };
                matches.Add(match);
            }
            _remainingPlayers = _remainingPlayers.Skip(numOfPlayers / 2).ToList();
            var round = new Round() { Matches = matches, RoundNumber = _currentTournament.Rounds.Count + 1 };
            _currentTournament.Rounds.Add(round);
            return round;
        }

        public bool TrySetMatchWinner(int matchNumber, string idOrNickname, out string errorMessage)
        {
            errorMessage = "";
            if (matchNumber < 0 || matchNumber > _currentTournament.Rounds.Last().Matches.Count)
            {
                errorMessage = "Invalid match number";
                return false;
            }
            if (!TryParsePlayer(idOrNickname, out Player player))
            {
                errorMessage = "Player does not exist";
                return false;
            }

            var match = _currentTournament.Rounds.Last().Matches[matchNumber - 1];
            if (!(match.Player1 == player) && !(match.Player2 == player))
            {
                errorMessage = "Player is not in the selected match";
                return false;
            }
            if (match.Winner != null)
            {
                errorMessage = "Match already has a winner";
                return false;
            }
            match.Winner = player;

            var winnerWinsAndLosses = _shadowPlayerWinsAndLosses[player.Id];
            _shadowPlayerWinsAndLosses[player.Id] = (winnerWinsAndLosses.WinsCount + 1, winnerWinsAndLosses.LossesCount);

            Player loserPlayer = match.Player1 == player ? match.Player2 : match.Player1!;
            var loserWinsAndLosses = _shadowPlayerWinsAndLosses[loserPlayer.Id];
            _shadowPlayerWinsAndLosses[loserPlayer.Id] = (loserWinsAndLosses.WinsCount, loserWinsAndLosses.LossesCount + 1);

            return true;
    }
}
