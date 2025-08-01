using Microsoft.EntityFrameworkCore;

namespace SolforbTestTask.Server.Data
{
    public class SolforbDBContext : DbContext
    {
        public SolforbDBContext()
        {
        }

        //public virtual DbSet<FileCSV> Files { get; set; }
        //public virtual DbSet<DataCSV> DataCSV { get; set; }
        //public virtual DbSet<Result> Results { get; set; }

        public SolforbDBContext(DbContextOptions<SolforbDBContext> options) : base(options)
        {
        }
    }
}
