using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentManager3000.Data;
using TournamentManager3000.Models;

namespace TournamentManager3000.Controllers.Helpers
{
    public class TournamentCreator
    {
        private readonly Player _dummyPlayer = new Player() { Nickname = "Dummy", Description = "Dummy player used to fill number of players to power of 2." };



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
                    newMatch = new Match() { Player1 = players[i], Player2 = (players[i + 1] == _dummyPlayer ? null : players[i + 1]) };
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
            for (int i = 0; i < numOfDummyPlayers * 2; i += 2)
            {
                players.Insert(i + 1, _dummyPlayer);
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
            var tournamentLoaded = tournamentContext.Tournaments.FirstOrDefault(t => t.Id == tournamentId);
            if (tournamentLoaded != null)
            {
                tournament = tournamentLoaded;
                return true;
            }
            else return false;
        }
    }
}
