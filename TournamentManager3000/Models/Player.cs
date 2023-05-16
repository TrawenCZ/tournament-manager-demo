using System.Text.Json.Serialization;

namespace TournamentManager3000.Models
{
    public class Player : IEntity
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Nickname { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; } = false;

        public int Wins { get; set; } = 0;
        public int Losses { get; set; } = 0;
        public int TournamentWins { get; set; } = 0;
        public int MatchesPlayed { get; set; } = 0;
    }
}
