using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            string currDirPath = Directory.GetCurrentDirectory();
            //DbPath = Path.Join(currDirPath, "Data", "DB", "tournamentManager.db");
            DbPath = "D:\\Data\\Codes\\PV178 Úvod do C#\\Project\\TournamentManager3000\\TournamentManager3000\\Data\\DB\\tournamentManager.db";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite($"Data Source={DbPath}");
    }
}
