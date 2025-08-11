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
    public class StorageServiceTest : BaseTest
    {
        [Fact]
        public async Task GetBalanceTest()
        {
            var logger = new Mock<ILogger<StorageService>>();

            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = GetSqliteInMemoryProviderOptions(connection);

            const string resName = "помидор";
            const string measureName = "кг";

            using (var ctx = new SolforbDBContext(options))
            {
                await ctx.Database.EnsureCreatedAsync();

                var r = new Resource
                {
                    Name = resName,
                    Status = 1
                };

                ctx.Resources.Add(r);

                var m = new Measurement
                {
                    Name = measureName,
                    Status = 1
                };

                ctx.Measurements.Add(m);

                await ctx.SaveChangesAsync();

                ctx.Balances.Add(new Balance
                {
                    MeasurementId = m.Id,
                    ResourceId = r.Id,
                    Count = 1
                });
                await ctx.SaveChangesAsync();
            }

            var service = new StorageService(() => new SolforbDBContext(options), logger.Object);

            var result = await service.GetBalanceAsync(new Radzen.Query
            {
                Top = 10
            });

            result.Success.Should().BeTrue();
            result.Data.Data.Should().BeEquivalentTo(
                [
                   new { Count = 1, Resource = new { Name = resName }, Measurement = new { Name = measureName } }
                ]);
        }

        [Fact]
        public async Task CreateReceiptDocumentTest()
        {
            var logger = new Mock<ILogger<StorageService>>();

            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = GetSqliteInMemoryProviderOptions(connection);

            const string resName = "помидор";
            const string measureName = "кг";

            long measureId;
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

                var m = new Measurement
                {
                    Name = measureName,
                    Status = 1
                };

                ctx.Measurements.Add(m);

                await ctx.SaveChangesAsync();

                resourceId = r.Id;
                measureId = m.Id;
            }

            var service = new StorageService(() => new SolforbDBContext(options), logger.Object);

            const string testNumber = "123";
            var testDate = DateOnly.FromDateTime(DateTime.Now).AddDays(-1);

            var result = await service.CreateReceiptDocumentAsync(new ReceiptDocumentDto
            {
                Number = testNumber,
                Date = testDate,
                ReceiptResources = [ new ReceiptResourceDto {
                    Count = 1,
                    Measurement = new MeasurementDto { Id = measureId },
                    Resource = new ResourceDto { Id = resourceId }
                }
                ]
            });

            result.Success.Should().BeTrue();

            using (var ctx = new SolforbDBContext(options))
            {

                ctx.Balances.ToList().Should().BeEquivalentTo(
                    [
                       new { Count = 1, ResourceId = resourceId, MeasurementId = measureId }
                    ]);
            }
        }

        [Fact]
        public async Task AddToBalanceTest()
        {
            var logger = new Mock<ILogger<StorageService>>();

            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = GetSqliteInMemoryProviderOptions(connection);

            const string resName = "помидор";
            const string measureName = "кг";

            long measureId;
            long resourceId;

            var firstDocDate = DateOnly.FromDateTime(DateTime.Now).AddDays(-30);
            const string firstDocNumber = "789";

            using (var ctx = new SolforbDBContext(options))
            {
                await ctx.Database.EnsureCreatedAsync();

                var r = new Resource
                {
                    Name = resName,
                    Status = 1
                };

                ctx.Resources.Add(r);

                var m = new Measurement
                {
                    Name = measureName,
                    Status = 1
                };

                ctx.Measurements.Add(m);

                await ctx.SaveChangesAsync();

                resourceId = r.Id;
                measureId = m.Id;

                ctx.Balances.Add(new Balance
                {
                    MeasurementId = measureId,
                    ResourceId = resourceId,
                    Count = 1,
                });

                var rd = new ReceiptsDocument { Date = firstDocDate, Number = firstDocNumber };
                ctx.ReceiptsDocuments.Add(rd);
                ctx.ReceiptsResources.Add(new ReceiptsResource { Count = 1, ReceiptsDocument = rd, MeasurementId = measureId, ResourceId = r.Id });
                await ctx.SaveChangesAsync();
            }

            var service = new StorageService(() => new SolforbDBContext(options), logger.Object);

            const string testNumber = "123";
            var testDate = DateOnly.FromDateTime(DateTime.Now).AddDays(-1);

            var result = await service.CreateReceiptDocumentAsync(new ReceiptDocumentDto
            {
                Number = testNumber,
                Date = testDate,
                ReceiptResources = [ new ReceiptResourceDto {
                    Count = 1,
                    Measurement = new MeasurementDto { Id = measureId },
                    Resource = new ResourceDto { Id = resourceId }
                }
                ]
            });

            result.Success.Should().BeTrue();

            using (var ctx = new SolforbDBContext(options))
            {
                ctx.Balances.ToList().Should().BeEquivalentTo(
                    [
                       new { Count = 2, ResourceId = resourceId, MeasurementId = measureId }
                    ]);
                ctx.ReceiptsDocuments.ToList().Should().BeEquivalentTo(
                    [
                       new 
                       {
                           Number = testNumber
                       },
                       new
                       {
                           Number = firstDocNumber
                       }
                    ]);
            }
        }

        [Fact]
        public async Task GetEmptyBalanceTest()
        {
            var logger = new Mock<ILogger<StorageService>>();

            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = GetSqliteInMemoryProviderOptions(connection);

            using (var ctx = new SolforbDBContext(options))
            {
                await ctx.Database.EnsureCreatedAsync();
            }

            var service = new StorageService(() => new SolforbDBContext(options), logger.Object);

            var result = await service.GetBalanceAsync(new Radzen.Query
            {
                Top = 10
            });

            result.Success.Should().BeTrue();
            result.Data.Data.Should().BeEmpty();
        }
    }
}
