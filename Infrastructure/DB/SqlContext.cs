using EntityProject.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Entity.Infrastructure.DB
{
    public class SqlContext : DbContext
    {
        private readonly SqlSettings _sqlSettings;
        
        public SqlContext(DbContextOptions<SqlContext> options, IOptions<SqlSettings> sqlSettings) : base(options)
        {
            _sqlSettings = sqlSettings.Value;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_sqlSettings.ConnectionString, sqlServerOptions => sqlServerOptions.CommandTimeout(5 * 60));
        }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}
