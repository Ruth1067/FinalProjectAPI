////////using System;
////////using System.Collections.Generic;
////////using System.Linq;
////////using System.Text;
////////using System.Threading.Tasks;

////////namespace FinalProject.Data
////////{
//////using Microsoft.EntityFrameworkCore;
//////using FinalProject.Core.Entities;
//////using global::FinalProject.Core.Entities;

//////namespace FinalProject.Infrastructure
//////{
//////    public class DataContext : DbContext
//////    {
//////        public DataContext(DbContextOptions<DataContext> options) : base(options)
//////        {
//////        }

//////        public DbSet<User> Users { get; set; } // Represents the Users table in the database

//////        protected override void OnModelCreating(ModelBuilder modelBuilder)
//////        {
//////            // You can configure your entity mappings here if needed
//////            modelBuilder.Entity<User>().ToTable("Users");
//////        }
//////    }
//////}

////////}
////using firstWebApi;
////using FinalProject.Core.Entities;
////using Microsoft.EntityFrameworkCore;
////using Microsoft.Extensions.DependencyInjection;

////namespace FinalProject
////{
////    public class DataContext : DbContext
////    {
////        public DataContext(DbContextOptions<DataContext> options) : base(options)
////        {
////        }
////        public DbSet<User> Users { get; set; }
////        public DbSet<Folder> Folders { get; set; }
////        // public DbSet<Baby> babies { get; set; }
////        //public DbSet<Nurse> nurses { get; set; }


////        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
////        //{
////        //    optionsBuilder.UseSqlServer(@"Server=bklae8to1zb68cllgzjv-mysql.services.clever-cloud.com;Database=bklae8to1zb68cllgzjv-mysql.services.clever-cloud.com");
////        //}
////        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
////        {
////            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=newDB");
////        }
////        //public void ConfigureServices(IServiceCollection services)
////        //{
////        //    services.AddDbContext<DbContext>(static options =>
////        //        options.UseMySql("Server=bklae8to1zb68cllgzjv-mysql.services.clever-cloud.com;Database=bklae8to1zb68cllgzjv-mysql.services.clever-cloud.com;",
////        //            new MySqlServerVersion(new Version(8, 0, 21))));
////        //}
////    }
////}

//using FinalProject.Core.Entities;
//using Microsoft.EntityFrameworkCore;
//using System.Reflection.Emit;

//namespace FinalProject
//{
//    public class DataContext : DbContext
//    {
//        public DataContext(DbContextOptions<DataContext> options) : base(options)
//        {
//        }

//        public DbSet<User> Users { get; set; }
//        public DbSet<Folder> Folders { get; set; }
//        public DbSet<FolderUser> FolderUsers { get; set; }


//        //protected override void OnModelCreating(ModelBuilder modelBuilder)
//        //{
//        //    modelBuilder.Entity<User>()
//        //        .HasMany(u => u.Folders)
//        //        .WithMany(f => f.Users)
//        //        .UsingEntity(j => j.ToTable("FolderUsers"));
//        //    base.OnModelCreating(modelBuilder);

//        //    modelBuilder.Entity<FolderUser>()
//        //        .HasKey(fu => new { fu.UserId, fu.FolderId });

//        //    modelBuilder.Entity<FolderUser>()
//        //        .HasOne(fu => fu.User)
//        //        .WithMany(u => u.FolderUsers)
//        //        .HasForeignKey(fu => fu.UserId);

//        //    modelBuilder.Entity<FolderUser>()
//        //        .HasOne(fu => fu.Folder)
//        //        .WithMany(f => f.FolderUsers)
//        //        .HasForeignKey(fu => fu.FolderId);
//        //}
//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            // הגדרת מפתח משולב לטבלת הקשר
//            modelBuilder.Entity<FolderUser>()
//                .HasKey(fu => new { fu.UserId, fu.FolderId });

//            // קשר בין FolderUser ל-User
//            modelBuilder.Entity<FolderUser>()
//                .HasOne(fu => fu.User)
//                .WithMany(u => u.FolderUsers)
//                .HasForeignKey(fu => fu.UserId);

//            // קשר בין FolderUser ל-Folder
//            modelBuilder.Entity<FolderUser>()
//                .HasOne(fu => fu.Folder)
//                .WithMany(f => f.FolderUsers)
//                .HasForeignKey(fu => fu.FolderId);

//            base.OnModelCreating(modelBuilder);
//        }


//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            //optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=SecondDB");
//        }
//    }
//}

using FinalProject.Core.Entities;
using Microsoft.EntityFrameworkCore;

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

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    // הגדרת מפתח משולב לטבלת הקשר בין משתמשים לקורסים
        //    modelBuilder.Entity<FolderUser>()
        //        .HasKey(fu => new { fu.UserId, fu.FolderId });

        //    modelBuilder.Entity<FolderUser>()
        //        .HasOne(fu => fu.User)
        //        .WithMany(u => u.FolderUsers)
        //        .HasForeignKey(fu => fu.UserId);

        //    modelBuilder.Entity<FolderUser>()
        //        .HasOne(fu => fu.Folder)
        //        .WithMany(f => f.FolderUsers)
        //        .HasForeignKey(fu => fu.FolderId);

        //    base.OnModelCreating(modelBuilder);
        //}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Folder>(entity =>
            {
                entity.Property(e => e.Title)
                      .IsRequired()
                      .HasColumnType("longtext");

                entity.Property(e => e.TeacherName)
                      .HasColumnType("longtext");

                entity.Property(e => e.description)
                      .HasColumnType("longtext");
            });

            // הגדרות נוספות, כמו הקשרים בטבלת FolderUser, אם יש
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


    }
}



//dsfghjkl;'lk;jhgfdsasdfghjkl;kjhgfdsasdfghjkl;kjlhgfhdsaddffghjkl;'kljhgfdsafdgfhghkjkl;s