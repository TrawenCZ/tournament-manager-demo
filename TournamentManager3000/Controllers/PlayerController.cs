using TournamentManager3000.Controllers.Helpers;
using TournamentManager3000.Data;
using TournamentManager3000.Models;

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

        public string MenuName(MenuInput _) => "Player menu";

        public string Help(MenuInput _)
        {
            return CommonMessages.HELP_HEADER +
                CommonMessages.HELP_OPTION +
                "'create-player <NICKNAME>' - creates a player with given unique nickname\n" +
                "'show-player <ID or NICKNAME>' - shows details about player with given ID/nickname\n" +
                "'list-players' - lists all stored players\n" +
                "'delete-player <ID or NICKNAME>' - deletes player with given ID/nickname\n" +
                CommonMessages.BACK_TO_MAIN;
        }

        public string Back(MenuInput _) => "Leaving Player menu";

        public string CreatePlayer(MenuInput input)
        {
            string message;
            if (!CommonMethods.CheckListLength(input, 1, 1, out message)) return message;

            Player existingPlayer;
            if (CommonMethods.TryParsePlayer(input[0], _tournamentContext, out existingPlayer)) return "Player with given nickname already exists. Try different one.";

            var newPlayer = new Player() { Nickname = input[0] };
            _tournamentContext.Players.Add(newPlayer);
            _tournamentContext.SaveChanges();
            return $"Player with nickname '{input[0]}' was successfuly created and has been assigned with ID '{newPlayer.Id}'.";
        }

        public string DeletePlayer(MenuInput input)
        {
            string message;
            if (!CommonMethods.CheckListLength(input, 1, 1, out message)) return message;

            Player playerToDelete;
            if (!CommonMethods.TryParsePlayer(input[0], _tournamentContext, out playerToDelete)) return "Player with given ID/nickname does not exist!";

            playerToDelete.IsDeleted = true;          // creating shadow entity to be able to show deleted players in tournaments
            _tournamentContext.Update(playerToDelete);
            _tournamentContext.SaveChanges();
            return $"Player with ID '{playerToDelete.Id}' and nickname '{playerToDelete.Nickname}'" + CommonMessages.SUCC_DEL;
        }

        public string ShowPlayer(MenuInput input)
        {
            string message;
            if (!CommonMethods.CheckListLength(input, 1, 1, out message)) return message;

            Player player;
            if (!CommonMethods.TryParsePlayer(input[0], _tournamentContext, out player)) return "Player with given ID/nickname does not exist!";

            return CommonMethods.BuildTableFromDictionary(new Dictionary<string, List<string>>()
            {
                {"ID", new List<string>() { player.Id.ToString() } },
                {"Nickname", new List<string>() { player.Nickname } },
                {"Total Wins", new List<string>() { player.Wins.ToString() } },
                {"Total Losses", new List<string>() {player.Losses.ToString() } },
                {"Tournament Wins", new List<string>() {player.TournamentWins.ToString() } },
                {"Matches played", new List<string>() { player.MatchesPlayed.ToString() } },
                {"Win / Loss ratio", new List<string>() { player.Losses == 0 ? "No losses yet" : (player.Wins / player.Losses).ToString() } },
                {"Win / Match ratio", new List<string>() { player.MatchesPlayed == 0 ? "No matches played yet" : (player.Wins / player.MatchesPlayed).ToString() } },
            });
        }

        public string ListPlayers(MenuInput input)
        {
            var allStoredPlayers = _tournamentContext.Players.Where(p => !p.IsDeleted).ToList();
            if (allStoredPlayers.Count == 0) return "No players stored yet.";
            return CommonMethods.ListPlayers(allStoredPlayers);
        }
    }
}
