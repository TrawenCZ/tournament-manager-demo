using Microsoft.EntityFrameworkCore;
using TournamentManager3000.Models;

namespace TournamentManager3000.Data
{
    public class TournamentContext : DbContext
    {
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Round> Rounds { get; set; }

        public string DbPath { get; }
        public TournamentContext()
        {
            DbPath = "D:\\Data\\Codes\\PV178 Úvod do C#\\Project\\TournamentManager3000\\TournamentManager3000\\Data\\DB\\tournamentManager.db";        // here goes location of your database file ('Data/DB/tournamentManager.db'),
                                                                                                                                                        // I'll leave here my path for reference, even in real life scenario I would
                                                                                                                                                        // never do this. I also tried to use relative path, but it didn't work for me,
                                                                                                                                                        // since program always run from different location, so I had to use absolute path.
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite($"Data Source={DbPath}");
    }
}
