using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentManager3000.Models
{
    public class Tournament : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public List<Round>? Rounds { get; private set; }

        public void Start(List<Player> players)
        {
            int numOfPlayers = players.Count;
            // closest power of 2 higher than numOfPlayers
            int numOfDummyPlayers = (int)Math.Pow(2, Math.Ceiling(Math.Log(numOfPlayers, 2))) - numOfPlayers;
            var dummyPlayer = new Player() { Id = -1, Nickname = "Dummy" };
            players = players.Concat(Enumerable.Repeat(dummyPlayer, numOfDummyPlayers)).ToList();
            Rounds = new List<Round>();
        }

        public void StartNewRound(List<Player> currentPlayers)
        {
            if (Rounds == null)
            {
                throw new InvalidOperationException("Tournament has not been started yet.");
            }

            var matches = new List<Match>();
            for (int i = 0; i < currentPlayers.Count; i += 2)
            {
                var match = new Match() { Player1 = currentPlayers[i], Player2 = currentPlayers[i + 1] };
                matches.Add(match);
            }
            var round = new Round() { Matches = matches, RoundNumber = Rounds.Count + 1 };
            Rounds.Add(round);
        }
    }
}
