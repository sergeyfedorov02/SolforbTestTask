using Microsoft.EntityFrameworkCore;
using SolforbTestTask.Server.Models.Entities;

namespace SolforbTestTask.Server.Data
{
    public class SolforbDBContext : DbContext
    {
        public SolforbDBContext()
        {
        }

        public virtual DbSet<Balance> Balances { get; set; }
        //public virtual DbSet<DataCSV> DataCSV { get; set; }
        //public virtual DbSet<Result> Results { get; set; }

        public SolforbDBContext(DbContextOptions<SolforbDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Настройка Balance
            builder.Entity<Balance>()
                .HasKey(r => r.Id);


            //// Настройка FileCSV
            //builder.Entity<FileCSV>()
            //    .HasIndex(f => f.FileName)  // индекс по имени файла
            //    .IsUnique(); // уникальный индекс


            //// Настройка DataCSV
            //builder.Entity<DataCSV>()
            //    .HasKey(d => d.Id);

            //builder.Entity<DataCSV>()
            //   .HasIndex(f => f.Date)  // индекс для сортировки по этому полю
            //   .IsUnique(false);

            //// для DataCSV(1 FileCSV->many DataCSV)
            //builder.Entity<FileCSV>()
            //    .HasMany(f => f.DataRecords)
            //    .WithOne(d => d.FileCSV)
            //    .HasForeignKey(d => d.FileId);


            //// Настройка Result
            //builder.Entity<Result>()
            //    .HasKey(r => r.Id);

            //// для Result (1 FileCSV -> 1 Result)
            //builder.Entity<FileCSV>()
            //    .HasOne(f => f.Result)
            //    .WithOne(r => r.FileCSV)
            //    .HasForeignKey<Result>(r => r.FileId);
        }
    }
}
