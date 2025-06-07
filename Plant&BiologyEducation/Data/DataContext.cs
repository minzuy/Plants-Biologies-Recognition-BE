using Microsoft.EntityFrameworkCore;
using Plant_BiologyEducation.Entity.Model;

namespace Plant_BiologyEducation.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<TakingTest> TakingTests { get; set; }
        public DbSet<Plant_BilogyAI> Plant_BiologyAIs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 

            // Test - User (Creator)
            modelBuilder.Entity<Test>()
                .HasOne(t => t.Creator)
                .WithMany(u => u.CreatedTests)
                .HasForeignKey(t => t.CreatorId)
                .OnDelete(DeleteBehavior.Restrict); // tránh cascade vòng

            // Question - Test
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Test)
                .WithMany(t => t.Questions)
                .HasForeignKey(q => q.TestId)
                .OnDelete(DeleteBehavior.Cascade); // xóa Test thì xóa câu hỏi

            // Taking - User
            modelBuilder.Entity<TakingTest>()
                .HasKey(t => new { t.UserId, t.TestId });

            modelBuilder.Entity<TakingTest>()
                .HasOne(t => t.User)
                .WithMany(u => u.Takings)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict); // KHÔNG dùng Cascade

            // Taking - Test
            modelBuilder.Entity<TakingTest>()
                .HasOne(t => t.Test)
                .WithMany(t => t.Takings)
                .HasForeignKey(t => t.TestId)
                .OnDelete(DeleteBehavior.Cascade); // xóa Test thì xóa bài làm
        }

    }
}
