using Microsoft.EntityFrameworkCore;

namespace SolvoTestTask.Server.Data
{
    public class SolvoDBContext : DbContext
    {
        public SolvoDBContext()
        {
        }

        //public virtual DbSet<FileCSV> Files { get; set; }
        //public virtual DbSet<DataCSV> DataCSV { get; set; }
        //public virtual DbSet<Result> Results { get; set; }

        public SolvoDBContext(DbContextOptions<SolvoDBContext> options) : base(options)
        {
        }
    }
}
