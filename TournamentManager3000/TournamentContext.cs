using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentManager3000.Models;

namespace TournamentManager3000
{
    public class TournamentContext : DbContext
    {
        public DbSet<Models.Tournament> Tournaments { get; set; }
        public DbSet<Models.Player> Players { get; set; }
        public DbSet<Models.Match> Matches { get; set; }
        public DbSet<Models.Round> Rounds { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Player>()
                .HasIndex(u => u.Nickname)
                .IsUnique();
        }
    }
}
