using MarvelApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarvelApp.Infrastructure
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {

        }

        public DbSet<Character> Characters { get; set; }
        public DbSet<RescueTeam> RescueTeams { get; set; }
    }
}
