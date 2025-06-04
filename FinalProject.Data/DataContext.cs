using FinalProject.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FinalProject
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<FolderUser> FolderUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FolderUser>()
                .HasKey(fu => new { fu.UserId, fu.FolderId });

            modelBuilder.Entity<FolderUser>()
                .HasOne(fu => fu.User)
                .WithMany(u => u.FolderUsers)
                .HasForeignKey(fu => fu.UserId);

            modelBuilder.Entity<FolderUser>()
                .HasOne(fu => fu.Folder)
                .WithMany(f => f.FolderUsers)
                .HasForeignKey(fu => fu.FolderId);

            base.OnModelCreating(modelBuilder);
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=SecondDB;");
        }
    }
}



