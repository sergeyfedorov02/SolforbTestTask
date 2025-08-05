using DataContracts;
using Microsoft.EntityFrameworkCore;
using Radzen;
using SolforbTestTask.Server.Data;
using SolforbTestTask.Server.Extensions;
using SolforbTestTask.Server.Models.Entities;
using System.Diagnostics.Metrics;

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
                        Measurement = new MeasurementInStorageDto
                        {
                            Name = v.Measurement.Name,
                        },
                        Resource = new ResourceInStorageDto 
                        {
                            Name = v.Resource.Name,
                        },
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

        public async Task<DataResultDto<GridResultDto<ReceiptDocumentDto>>> GetReceiptAsync(Query query)
        {
            try
            {
                await using var context = ContextProvider();

                var receipts = await context.ReceiptsDocuments
                    .Include(d => d.ReceiptsResources)
                    .ThenInclude(r => r.Resource)
                    .Include(d => d.ReceiptsResources)
                    .ThenInclude(d => d.Measurement)
                    .GetDataAsync(query, "Id asc");


                return DataResultDto<GridResultDto<ReceiptDocumentDto>>.CreateFromData(new GridResultDto<ReceiptDocumentDto>
                {
                    Data = [.. receipts.Value.Select(v => new ReceiptDocumentDto
                    {
                        Number = v.Number,
                        Date = v.Date,
                        ReceiptResources = v.ReceiptsResources.Select(rr => new ReceiptResourceDto {
                            Resource = new ResourceDto 
                            {
                                Id = rr.Resource.Id,
                                Name = rr.Resource.Name,
                                Status = rr.Resource.Status
                            },
                            Measurement = new MeasurementDto
                            {
                                Id = rr.Measurement.Id,
                                Name = rr.Measurement.Name,
                                Status = rr.Measurement.Status
                            },
                            Count = rr.Count
                        }).ToList()
                    })],
                    Count = receipts.Count
                });
            }
            catch (Exception ex)
            {
                return DataResultDto<GridResultDto<ReceiptDocumentDto>>.CreateFromException(ex);
            }
        }

        public async Task<ResultDto> CreateReceiptDocumentAsync(ReceiptDocumentDto receiptDocumentDto)
        {
            await using var context = ContextProvider();
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                // ReceiptDocument
                var receiptDocument = new ReceiptsDocument
                {
                    Number = receiptDocumentDto.Number,
                    Date = receiptDocumentDto.Date.ToUniversalTime()  // конвертация в UTC
                };

                if (context.ReceiptsDocuments.FirstOrDefault(r => r.Number == receiptDocument.Number) != null)
                {
                    throw new ArgumentException("Документ поступления с таким номером уже существует");
                }

                context.ReceiptsDocuments.Add(receiptDocument);
                await context.SaveChangesAsync();

                // ReceiptResource
                foreach (ReceiptResourceDto receiptResourceDto in receiptDocumentDto.ReceiptResources)
                {
                    // Проверка для Ресурса
                    var resourceExists = await context.Resources
                        .AnyAsync(r => r.Id == receiptResourceDto.Resource.Id && r.Status == 1);

                    if (!resourceExists)
                    {
                        throw new ArgumentException($"Ресурс с Id = {receiptResourceDto.Resource.Id} не найден");
                    }

                    // Проверка для Единицы Измерения
                    var measurementExists = await context.Measurements
                        .AnyAsync(m => m.Id == receiptResourceDto.Measurement.Id && m.Status == 1);

                    if (!measurementExists)
                    {
                        throw new ArgumentException($"Единица измерения с Id = {receiptResourceDto.Measurement.Id} не найдена");
                    }

                    var receiptResource = new ReceiptsResource
                    {
                        DocumentId = receiptDocument.Id,
                        ResourceId = receiptResourceDto.Resource.Id,
                        MeasurementId = receiptResourceDto.Measurement.Id,
                        Count = receiptResourceDto.Count
                    };

                    context.ReceiptsResources.Add(receiptResource);
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ResultDto.CreateOk();
            }
            catch (ArgumentException ex)
            {
                await transaction.RollbackAsync();
                return ResultDto.CreateFromException(ex);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Logger.LogError(ex, "Ошибка при сохранении документа поступления");
                return ResultDto.CreateFromException(new Exception("Ошибка при сохранении документа поступления", ex));
            }
        }
    }
}
