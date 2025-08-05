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
        public virtual DbSet<Resource> Resources { get; set; }
        public virtual DbSet<Measurement> Measurements { get; set; }
        public virtual DbSet<ReceiptsDocument> ReceiptsDocuments { get; set; }
        public virtual DbSet<ReceiptsResource> ReceiptsResources { get; set; }
        public virtual DbSet<ShipmentsDocument> ShipmentsDocuments { get; set; }
        public virtual DbSet<ShipmentsResource> ShipmentsResources { get; set; }

        public SolforbDBContext(DbContextOptions<SolforbDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Настройка Balance
            builder.Entity<Balance>()
                .HasKey(r => r.Id);

            // Настройка ReceiptsDocument
            builder.Entity<ReceiptsDocument>()
                 .HasKey(d => d.Id);

            builder.Entity<ReceiptsDocument>()
               .HasIndex(f => f.Number) // индекс для сортировки по этому полю
               .IsUnique(true);

            // Настройка ReceiptsResource
            builder.Entity<ReceiptsResource>()
                .HasKey(r => r.Id);

            builder.Entity<ReceiptsResource>()
                .HasIndex(f => f.DocumentId) // индекс для сортировки по этому полю
                .IsUnique(true);

            // Настройка Resource
            builder.Entity<Resource>()
                .HasIndex(r => r.Name)
                .IsUnique();

            // Настройка Measurement
            builder.Entity<Measurement>()
                .HasIndex(r => r.Name)
                .IsUnique();
        }
    }
}
