using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SolforbTestTask.Server.Data;

namespace SolforbTests
{
    public class BaseTest
    {
        protected static DbContextOptions<SolforbDBContext> GetSqliteInMemoryProviderOptions(SqliteConnection connection)
        {
            return new DbContextOptionsBuilder<SolforbDBContext>().UseSqlite(connection).Options;
        }
    }
}
