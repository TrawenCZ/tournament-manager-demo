namespace TournamentManager3000.Models
{
    public class Round : IEntity
    {
        public int Id { get; set; }
        public int RoundNumber { get; set; }

        public int TournamentId { get; set; }

        public List<Match> Matches { get; init; }
    }
}
