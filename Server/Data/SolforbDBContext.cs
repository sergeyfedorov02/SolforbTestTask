using Microsoft.EntityFrameworkCore;
using SolforbTestTask.Server.Models.Entities;
using System.Reflection.Emit;

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

            builder.Entity<Balance>()
                .Property(b => b.Count)
                .IsConcurrencyToken();  // токен параллелизма (когда несколько пользователей вносят изменения одновременно)

            // Настройка ReceiptsDocument
            builder.Entity<ReceiptsDocument>()
                 .HasKey(d => d.Id);

            builder.Entity<ReceiptsDocument>()
               .HasIndex(f => f.Number) // индекс для сортировки по этому полю
               .IsUnique(true);
            builder.Entity<ReceiptsDocument>()
                .Property(r => r.Number)
                .HasMaxLength(64);

            // Настройка ReceiptsResource
            builder.Entity<ReceiptsResource>()
                .HasKey(r => r.Id);

            builder.Entity<ReceiptsResource>()
                .HasIndex(f => new { f.DocumentId, f.MeasurementId, f.ResourceId }) // индекс для обеспечения уникальности 
                .IsUnique(true);

            // Настройка Resource
            builder.Entity<Resource>()
                .HasIndex(r => r.Name)
                .IsUnique();
            builder.Entity<Resource>()
                .Property(r => r.Name)
                .HasMaxLength(64);

            // Настройка Measurement
            builder.Entity<Measurement>()
                .HasIndex(r => r.Name)
                .IsUnique();
            builder.Entity<Measurement>()
                .Property(r => r.Name)
                .HasMaxLength(64);
        }
    }
}
