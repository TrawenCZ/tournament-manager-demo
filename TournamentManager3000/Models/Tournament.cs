namespace TournamentManager3000.Models
{
    public class Tournament : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Round> Rounds { get; set; } = new List<Round>();
    }
}
