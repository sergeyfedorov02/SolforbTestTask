using DataContracts;
using Microsoft.EntityFrameworkCore;
using Radzen;
using SolforbTestTask.Server.Data;
using SolforbTestTask.Server.Extensions;

namespace SolforbTestTask.Server.Services
{
    /// <summary>
    /// Сервис для получения данных из БД
    /// </summary>
    public class StorageService : IStorageService
    {
        /// <summary>
        /// // Контекст - для связи объектной модели и БД (напрямую с БД не работают)
        /// </summary>
        private Func<SolforbDBContext> ContextProvider { get; }

        private ILogger<StorageService> Logger { get; }

        public StorageService(Func<SolforbDBContext> provider, ILogger<StorageService> logger)
        {
            ContextProvider = provider;
            Logger = logger;
        }

        public async Task<DataResultDto<GridResultDto<BalanceDto>>> GetBalanceAsync(Query query)
        {
            try
            {
                await using var context = ContextProvider();

                var result = await context.Balances.Include(o => o.Resource).Include(o => o.Measurement).GetDataAsync(query, "Id asc");

                return DataResultDto<GridResultDto<BalanceDto>>.CreateFromData(new GridResultDto<BalanceDto>
                {
                    Data = [.. result.Value.Select(v => new BalanceDto
                    {
                        MeasurementName = v.Measurement.Name,
                        ResourceName = v.Resource.Name,
                        Count = v.Count
                    })],
                    Count = result.Count
                });       
            }
            catch (Exception ex)
            {
                return DataResultDto<GridResultDto<BalanceDto>>.CreateFromException(ex);
            }
        }
    }
}
