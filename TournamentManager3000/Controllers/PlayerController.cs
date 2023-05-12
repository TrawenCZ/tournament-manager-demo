using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TournamentManager3000.Controllers.Helpers;
using TournamentManager3000.Models;
using TournamentManager3000.UI;

namespace TournamentManager3000.Controllers
{
    using MenuInput = List<string>;
    public class PlayerController
    {
        private readonly TournamentContext _tournamentContext;


        public PlayerController(TournamentContext tournamentContext) 
        {
            _tournamentContext = tournamentContext;
        }

        public string MenuName(MenuInput input) => "Player";

        public string Help(MenuInput input)
        {
            return CommonMessages.HELP_HEADER +
                CommonMessages.HELP_OPTION +
                "'create-player <NICKNAME> <DESCRIPTION>?' - creates a player with given unique nickname and possible description\n" +
                "'show-player <ID or NICKNAME>' - shows details about player with given ID/nickname\n" +
                "'list-players' - lists all stored players\n" +
                "'delete-player <ID or NICKNAME>' - deletes player with given ID/nickname\n";
        }

        public string Exit(MenuInput _) => "Leaving Player menu";

        public string CreatePlayer(MenuInput input)
        {
            var message = "";
            if (!CommonMethods.CheckListLength(input, 1, 2, out message)) return message;

            var existingPlayer = new Player();
            if (CommonMethods.TryParsePlayer(input[0], _tournamentContext, out existingPlayer)) return "Player with given nickname already exists. Try different one.";

            var newPlayer = new Player() { Nickname = input[0], Description = (input.Count == 2 ? input[1] : null) };
            _tournamentContext.Players.Add(newPlayer);
            _tournamentContext.SaveChanges();
            return $"Player with nickname '{input[0]}' was successfuly created and has been assigned with ID '{newPlayer.Id}'.";
        }

        public string DeletePlayer(MenuInput input)
        {
            var message = "";
            if (!CommonMethods.CheckListLength(input, 1, 1, out message)) return message;

            var playerToDelete = new Player();
            if (!CommonMethods.TryParsePlayer(input[0], _tournamentContext, out playerToDelete)) return "Player with given ID/nickname does not exist!";

            playerToDelete.IsDeleted = true;
            _tournamentContext.Update(playerToDelete);
            _tournamentContext.SaveChanges();
            return $"Player with ID '{playerToDelete.Id}' and nickname '{playerToDelete.Nickname}'" + CommonMessages.SUCC_DEL;
        }

        public string ShowPlayer(MenuInput input)
        {
            var message = "";
            if (!CommonMethods.CheckListLength(input, 1, 2, out message)) return message;

            var player = new Player();
            if (!CommonMethods.TryParsePlayer(input[0], _tournamentContext, out player)) return "Player with given ID/nickname does not exist!";

            return CommonMethods.BuildTableFromDictionary(new Dictionary<string, List<string>>()
            {
                {"ID", new List<string>() { player.Id.ToString() } },
                {"Nickname", new List<string>() { player.Nickname } },
                {"Description", new List<string>() { player.Description != null ? player.Description : CommonMessages.NO_DESCR } },
                {"Total Wins", new List<string>() { player.Wins.ToString() } },
                {"Total Losses", new List<string>() {player.Losses.ToString() } },
                {"Matches played", new List<string>() { player.MatchesPlayed.ToString() } },
                {"Win / Loss ratio", new List<string>() { player.Losses == 0 ? "No losses yet" : (player.Wins / player.Losses).ToString() } },
                {"Win / Match ratio", new List<string>() { player.MatchesPlayed == 0 ? "No matches played yet" : (player.Wins / player.MatchesPlayed).ToString() } },
            });
        }

        public string ListPlayers(MenuInput input)
        {
            var allStoredPlayers = _tournamentContext.Players.ToList();
            if (allStoredPlayers.Count == 0) return "No players stored yet.";
            return CommonMethods.BuildTableFromDictionary(new Dictionary<string, List<string>>()
            {
                {"ID", allStoredPlayers.Select(p => p.Id.ToString()).ToList() },
                {"Nickname", allStoredPlayers.Select(p => p.Nickname).ToList() },
                {"Description", allStoredPlayers.Select(p => p.Description != null ? p.Description : CommonMessages.NO_DESCR).ToList() },
                {"Total Wins", allStoredPlayers.Select(p => p.Wins.ToString()).ToList() },
                {"Total Losses", allStoredPlayers.Select(p => p.Losses.ToString()).ToList() },
                {"Matches played", allStoredPlayers.Select(p => p.MatchesPlayed.ToString()).ToList() },
                {"Win / Loss ratio", allStoredPlayers.Select(p => p.Losses == 0 ? "No losses yet" : (p.Wins / p.Losses).ToString()).ToList() },
                {"Win / Match ratio", allStoredPlayers.Select(p => p.MatchesPlayed == 0 ? "No matches played yet" : (p.Wins / p.MatchesPlayed).ToString()).ToList() },
            });
        }
    }
}
