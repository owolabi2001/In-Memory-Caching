using LearningRedisCaching.Model;
using Microsoft.EntityFrameworkCore;

namespace LearningRedisCaching
{
    public class ApplicationDbContext:DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }


        public DbSet<Office> Office { get; set; }
        public DbSet<ArticleMatrix>? ArticleMatrix { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
