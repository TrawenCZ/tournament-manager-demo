namespace TournamentManager3000.Models
{
    public class Match : IEntity
    {
        public int Id { get; set; }
        public int Player1Id { get; set; }
        public Player Player1 { get; set; }
        public int? Player2Id { get; set; }
        public Player? Player2 { get; set; }


        public int RoundId { get; set; }        // these foreign keys are here for cascade delete purposes (EF Core doesn't support cascade delete without them)


        public int? WinnerId { get; set; }
        public Player? Winner { get; set; }
    }
}
