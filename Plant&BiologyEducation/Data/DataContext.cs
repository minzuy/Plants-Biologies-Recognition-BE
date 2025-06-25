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
        public DbSet<ManageBook> ManageBooks { get; set; }
        public DbSet<ManageChapter> ManageChapters { get; set; }
        public DbSet<ManageLesson> ManageLessons { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Plant_Biology_Animals> Plant_Biology_Animals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Khóa chính cho các bảng chính
            modelBuilder.Entity<User>().HasKey(u => u.User_Id);
            modelBuilder.Entity<Book>().HasKey(b => b.Book_Id);
            modelBuilder.Entity<Chapter>().HasKey(c => c.Chapter_Id);
            modelBuilder.Entity<Lesson>().HasKey(l => l.Lesson_Id);
            modelBuilder.Entity<Plant_Biology_Animals>().HasKey(p => p.Id);

            // Khóa chính + thiết lập quan hệ cho bảng trung gian ManageBook
            modelBuilder.Entity<ManageBook>()
                .HasKey(mb => new { mb.User_Id, mb.Book_Id });

            modelBuilder.Entity<ManageBook>()
                .HasOne(mb => mb.User)
                .WithMany(u => u.ManagedBooks)
                .HasForeignKey(mb => mb.User_Id);

            modelBuilder.Entity<ManageBook>()
                .HasOne(mb => mb.Book)
                .WithMany(b => b.ManagedBy)
                .HasForeignKey(mb => mb.Book_Id);

            // ManageChapter
            modelBuilder.Entity<ManageChapter>()
                .HasKey(mc => new { mc.User_Id, mc.Chapter_Id });

            modelBuilder.Entity<ManageChapter>()
                .HasOne(mc => mc.User)
                .WithMany(u => u.ManagedChapters)
                .HasForeignKey(mc => mc.User_Id);

            modelBuilder.Entity<ManageChapter>()
                .HasOne(mc => mc.Chapter)
                .WithMany(c => c.ManagedBy)
                .HasForeignKey(mc => mc.Chapter_Id);

            // ManageLesson
            modelBuilder.Entity<ManageLesson>()
                .HasKey(ml => new { ml.User_Id, ml.Lesson_Id });

            modelBuilder.Entity<ManageLesson>()
                .HasOne(ml => ml.User)
                .WithMany(u => u.ManagedLessons)
                .HasForeignKey(ml => ml.User_Id);

            modelBuilder.Entity<ManageLesson>()
                .HasOne(ml => ml.Lesson)
                .WithMany(l => l.ManagedBy)
                .HasForeignKey(ml => ml.Lesson_Id);

            // Quan hệ 1-n Chapter - Lesson
            modelBuilder.Entity<Lesson>()
                .HasOne(l => l.Chapter)
                .WithMany(c => c.Lessons)
                .HasForeignKey(l => l.Chapter_Id);

            // Quan hệ 1-n Book - Chapter
            modelBuilder.Entity<Chapter>()
                .HasOne(c => c.Book)
                .WithMany(b => b.Chapters)
                .HasForeignKey(c => c.Book_Id);

            // Quan hệ 1-n: Lesson - Plant_Biology_Animals
            modelBuilder.Entity<Plant_Biology_Animals>()
                .HasOne(p => p.Lesson)
                .WithMany(l => l.RelatedSpecies)
                .HasForeignKey(p => p.LessonId)
                .OnDelete(DeleteBehavior.Cascade); // hoặc Restrict nếu bạn không muốn xóa cascade
        }

    }
}