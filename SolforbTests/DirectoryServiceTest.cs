using DataContracts;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Moq;
using SolforbTestTask.Server.Data;
using SolforbTestTask.Server.Models.Entities;
using SolforbTestTask.Server.Services;

namespace SolforbTests
{
    public class DirectoryServiceTest : BaseTest
    {
        [Fact]
        public async Task CreateResourceTest()
        {
            var logger = new Mock<ILogger<DirectoryService>>();

            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = GetSqliteInMemoryProviderOptions(connection);

            using (var ctx = new SolforbDBContext(options))
            {
                await ctx.Database.EnsureCreatedAsync();
            }

            var service = new DirectoryService(() => new SolforbDBContext(options), logger.Object);

            var result = await service.CreateResourceAsync("помидор");

            result.Success.Should().BeTrue();

            using (var ctx = new SolforbDBContext(options))
            {
                var resources = ctx.Resources.ToList();
                resources.Should().BeEquivalentTo(
                [
                     new
                     {
                         Name = "помидор",
                         Status = 1
                     }
                ]);
            }
        }

        [Fact]
        public async Task DeleteNotExistingResourceFailTest()
        {
            var logger = new Mock<ILogger<DirectoryService>>();

            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = GetSqliteInMemoryProviderOptions(connection);

            using (var ctx = new SolforbDBContext(options))
            {
                await ctx.Database.EnsureCreatedAsync();
            }

            var service = new DirectoryService(() => new SolforbDBContext(options), logger.Object);

            var result = await service.DeleteResourceAsync(1000);

            result.Success.Should().BeFalse();
            result.Exception.Message.Should().Be("Ресурс не найден");
        }

        [Fact]
        public async Task DeleteExistingNotUsedResourceTest()
        {
            var logger = new Mock<ILogger<DirectoryService>>();

            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = GetSqliteInMemoryProviderOptions(connection);

            const string resName = "помидор";

            long resourceId;
            using (var ctx = new SolforbDBContext(options))
            {
                await ctx.Database.EnsureCreatedAsync();

                var r = new Resource
                {
                    Name = resName,
                    Status = 1
                };
                
                ctx.Resources.Add(r);
                await ctx.SaveChangesAsync();

                resourceId = r.Id;

                ctx.Resources.ToList().Should().BeEquivalentTo(
                    [
                      new { Name = resName, Status = 1 }
                    ]);
            }

            var service = new DirectoryService(() => new SolforbDBContext(options), logger.Object);

            var result = await service.DeleteResourceAsync(resourceId);

            result.Success.Should().BeTrue();

            using (var ctx = new SolforbDBContext(options))
            {
                ctx.Resources.Count().Should().Be(0);
            }
        }

        [Fact]
        public async Task GetResourceTest()
        {
            var logger = new Mock<ILogger<DirectoryService>>();

            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = GetSqliteInMemoryProviderOptions(connection);

            const string resName1 = "помидор";
            const string resName2 = "огурец";

            using (var ctx = new SolforbDBContext(options))
            {
                await ctx.Database.EnsureCreatedAsync();

                ctx.Resources.AddRange(new Resource
                {
                    Name = resName1,
                    Status = 1
                }, new Resource
                {
                    Name = resName2,
                    Status = 1
                });

                await ctx.SaveChangesAsync();
            }

            var service = new DirectoryService(() => new SolforbDBContext(options), logger.Object);

            var result = await service.GetResourceAsync(new Radzen.Query
            {
               Top = 10
            }, 1);

            result.Success.Should().BeTrue();
            result.Data.Data.Should().BeEquivalentTo(
                [
                   new { Name = resName1, Status = 1 },
                   new { Name = resName2, Status = 1 }
                ]);
            
        }

        [Fact]
        public async Task ArchiveResourceTest()
        {
            var logger = new Mock<ILogger<DirectoryService>>();

            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = GetSqliteInMemoryProviderOptions(connection);

            const string resName1 = "помидор";
            const string resName2 = "огурец";

            long resArchiveId;

            using (var ctx = new SolforbDBContext(options))
            {
                await ctx.Database.EnsureCreatedAsync();

                var r = new Resource
                {
                    Name = resName2,
                    Status = 1
                };

                ctx.Resources.AddRange(new Resource
                {
                    Name = resName1,
                    Status = 1
                }, r);

                await ctx.SaveChangesAsync();

                resArchiveId = r.Id;
            }

            var service = new DirectoryService(() => new SolforbDBContext(options), logger.Object);

            var result = await service.ArchiveResourceAsync(new ResourceDto {  Id = resArchiveId });

            result.Success.Should().BeTrue();

            using (var ctx = new SolforbDBContext(options))
            {
                ctx.Resources.ToList().Should().BeEquivalentTo(
                    [
                        new
                        {
                             Name = resName1,
                             Status = 1
                        },
                        new
                        {
                             Name = resName2,
                             Status = 2
                        }
                    ]);
            }
        }

        [Fact]
        public async Task CreateDuplicateResourceFailTest()
        {
            var logger = new Mock<ILogger<DirectoryService>>();

            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = GetSqliteInMemoryProviderOptions(connection);

            const string resName = "помидор";

            using (var ctx = new SolforbDBContext(options))
            {
                await ctx.Database.EnsureCreatedAsync();

                ctx.Resources.Add(new Resource
                {
                    Name = resName,
                    Status = 1
                });
                await ctx.SaveChangesAsync();
            }

            var service = new DirectoryService(() => new SolforbDBContext(options), logger.Object);

            var result = await service.CreateResourceAsync(resName);

            result.Success.Should().BeFalse();
            result.Exception.Message.Should().Be("Ресурс с таким именем уже существует");

            using (var ctx = new SolforbDBContext(options))
            {
                var resources = ctx.Resources.ToList();
                resources.Should().BeEquivalentTo(
                [
                     new
                     {
                         Name = resName,
                         Status = 1
                     }
                ]);
            }
        }
    }
}
