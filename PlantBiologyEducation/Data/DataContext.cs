using Microsoft.EntityFrameworkCore;
using Plant_BiologyEducation.Entity.Model;
using PlantBiologyEducation.Entity.Model;

namespace Plant_BiologyEducation.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Plant_Biology_Animals> Plant_Biology_Animals { get; set; }
        public DbSet<AccessBook> AccessBooks { get; set; }
        public DbSet<AccessLesson> AccessLessons { get; set; }
        public DbSet<AccessBiology> AccessBiologies { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Khóa chính cho các bảng chính
            modelBuilder.Entity<User>().HasKey(u => u.User_Id);
            modelBuilder.Entity<Book>().HasKey(b => b.Book_Id);
            modelBuilder.Entity<Chapter>().HasKey(c => c.Chapter_Id);
            modelBuilder.Entity<Lesson>().HasKey(l => l.Lesson_Id);
            modelBuilder.Entity<Plant_Biology_Animals>().HasKey(p => p.Id);     

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

            // AccessBook
            modelBuilder.Entity<AccessBook>()
                .HasKey(ab => ab.AccessBook_Id);

            modelBuilder.Entity<AccessBook>()
                .HasOne(ab => ab.User)
                .WithMany()
                .HasForeignKey(ab => ab.User_Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AccessBook>()
                .HasOne(ab => ab.Book)
                .WithMany()
                .HasForeignKey(ab => ab.Book_Id)
                .OnDelete(DeleteBehavior.Cascade);

            // AccessLesson
            modelBuilder.Entity<AccessLesson>()
                .HasKey(al => al.AccessLesson_Id);

            modelBuilder.Entity<AccessLesson>()
                .HasOne(al => al.User)
                .WithMany()
                .HasForeignKey(al => al.User_Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AccessLesson>()
                .HasOne(al => al.Lesson)
                .WithMany()
                .HasForeignKey(al => al.Lesson_Id)
                .OnDelete(DeleteBehavior.Cascade);

            // AccessBiology
            modelBuilder.Entity<AccessBiology>()
                .HasKey(ab => ab.AccessBiology_Id);

            modelBuilder.Entity<AccessBiology>()
                .HasOne(ab => ab.User)
                .WithMany()
                .HasForeignKey(ab => ab.User_Id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AccessBiology>()
                .HasOne(ab => ab.Oject)
                .WithMany()
                .HasForeignKey(ab => ab.Oject_Id)
                .OnDelete(DeleteBehavior.Cascade);


        }

    }
}