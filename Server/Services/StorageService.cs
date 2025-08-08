using DataContracts;
using Microsoft.EntityFrameworkCore;
using Radzen;
using SolforbTestTask.Server.Data;
using SolforbTestTask.Server.Extensions;
using SolforbTestTask.Server.Models.Entities;

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

        public async Task<DataResultDto<GridResultDto<ReceiptDocumentItemDto>>> GetReceiptDocumentItemsAsync(FilterReceiptItemsDto query)
        {
            try
            {
                await using var context = ContextProvider();

                var receiptsQuery = context.ReceiptsDocuments
                    .SelectMany(d => d.ReceiptsResources.Select(r => new ReceiptDocumentItemDto
                    {
                        Id = d.Id,
                        Number = d.Number,
                        Date = d.Date,
                        ReceptItem = new ReceiptResourceDto
                        {
                            Resource = new ResourceDto
                            {
                                Id = r.Resource.Id,
                                Name = r.Resource.Name,
                                Status = r.Resource.Status
                            },
                            Measurement = new MeasurementDto
                            {
                                Id = r.Measurement.Id,
                                Name = r.Measurement.Name,
                                Status = r.Measurement.Status
                            },
                            Count = r.Count,
                        }
                    }));

                var receiptWithNoItems = context.ReceiptsDocuments.Where(d => !d.ReceiptsResources.Any())
                    .Select(d => new ReceiptDocumentItemDto
                    {
                        Id = d.Id,
                        Number = d.Number,
                        Date = d.Date,
                        ReceptItem = new ReceiptResourceDto
                        {
                            Resource = new ResourceDto
                            {
                                Id = 0,
                                Name = "",
                                Status = 1
                            },

                            Measurement = new MeasurementDto
                            {
                                Id = 0,
                                Name = "",
                                Status = 1
                            },
                            Count = 0
                        }
                    });

                // Применение фильтров
                var queryAll = receiptsQuery.Union(receiptWithNoItems);

                if (query.FromDate != null)
                {
                    queryAll = queryAll.Where(d => d.Date >= query.FromDate.Value);
                }

                if (query.ToDate != null)
                {
                    queryAll = queryAll.Where(d => d.Date <= query.ToDate.Value);
                }

                if (query.DocumentNumbers != null && query.DocumentNumbers.Count != 0)
                {
                    queryAll = queryAll.Where(d => query.DocumentNumbers.Contains(d.Number));
                }

                if (query.ResourceIds != null && query.ResourceIds.Count != 0)
                {
                    queryAll = queryAll.Where(d => query.ResourceIds.Contains(d.ReceptItem.Resource.Id));
                }

                if (query.MeasurementIds != null && query.MeasurementIds.Count != 0)
                {
                    queryAll = queryAll.Where(d => query.MeasurementIds.Contains(d.ReceptItem.Measurement.Id));
                }

                IQueryable<ReceiptDocumentItemDto> receiptsWithEmptyQuery = queryAll.OrderBy(d => d.Id);

                var count = await receiptsWithEmptyQuery.CountAsync();

                if (query.Skip != null)
                {
                    receiptsWithEmptyQuery = receiptsWithEmptyQuery.Skip(query.Skip.Value);
                }

                if (query.Top != null)
                {
                    receiptsWithEmptyQuery = receiptsWithEmptyQuery.Take(query.Top.Value);
                }

                var items = await receiptsWithEmptyQuery.ToListAsync();

                return DataResultDto<GridResultDto<ReceiptDocumentItemDto>>.CreateFromData(new GridResultDto<ReceiptDocumentItemDto>
                {
                    Data = items,
                    Count = count
                });
            }
            catch (Exception ex)
            {
                return DataResultDto<GridResultDto<ReceiptDocumentItemDto>>.CreateFromException(ex);
            }
        }

        public async Task<ResultDto> CreateReceiptDocumentAsync(ReceiptDocumentDto receiptDocumentDto)
        {
            const int maxRetryAttempts = 3;
            var retryCount = 0;
            bool success = false;

            while (retryCount < maxRetryAttempts && !success)
            {
                await using var context = ContextProvider();
                await using var transaction = await context.Database.BeginTransactionAsync();

                try
                {
                    // ReceiptDocument
                    var receiptDocument = new ReceiptsDocument
                    {
                        Number = receiptDocumentDto.Number,
                        Date = receiptDocumentDto.Date
                    };

                    if (await context.ReceiptsDocuments.AnyAsync(r => r.Number == receiptDocument.Number))
                    {
                        throw new ArgumentException("Документ поступления с таким номером уже существует");
                    }

                    context.ReceiptsDocuments.Add(receiptDocument);
                    await context.SaveChangesAsync();

                    // ReceiptResource
                    foreach (var receiptResourceDto in receiptDocumentDto.ReceiptResources ?? [])
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

                        // Обновляем Balance
                        var existingBalance = context.Balances.FirstOrDefault(b =>
                            b.ResourceId == receiptResource.ResourceId &&
                            b.MeasurementId == receiptResource.MeasurementId);

                        // Запись существует -> обновляем
                        if (existingBalance != null)
                        {
                            existingBalance.Count += receiptResource.Count;
                            context.Balances.Update(existingBalance);
                        }
                        else
                        {
                            // не существует -> создаем
                            var newBalance = new Balance
                            {
                                ResourceId = receiptResource.ResourceId,
                                MeasurementId = receiptResource.MeasurementId,
                                Count = receiptResource.Count
                            };

                            context.Balances.Add(newBalance);
                        }
                    }

                    await context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    success = true;

                    return ResultDto.CreateOk();
                }
                catch (DbUpdateConcurrencyException conEx)
                {
                    await transaction.RollbackAsync();
                    retryCount++;

                    if (retryCount >= maxRetryAttempts)
                    {
                        Logger.LogWarning(conEx, $"Не удалось сохранить изменения после {maxRetryAttempts} попыток");
                        return ResultDto.CreateFromException(new Exception("Не удалось сохранить документ из-за конфликта параллельных изменений"));
                    }

                    await Task.Delay(100 * retryCount);
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

            return ResultDto.CreateFromException(new Exception("Неизвестная ошибка при создании документа поступления"));
        }

        /// <summary>
        /// Получение номеров документов для фильтров ReceiptsDocument
        /// </summary>
        /// <returns></returns>
        public async Task<DataResultDto<List<string>>> GetReceiptsDocumentNumbersFilterAsync()
        {
            try
            {
                await using var context = ContextProvider();

                var documentNumbers = await context.ReceiptsDocuments
                    .Where(d => d.Number != null && d.Number != "")
                    .Select(d => d.Number)
                    .ToListAsync();

                return DataResultDto<List<string>>.CreateFromData(documentNumbers);
            }
            catch (Exception ex)
            {
                return DataResultDto<List<string>>.CreateFromException(ex);
            }
        }

        /// <summary>
        /// Получение Resources для фильтров ReceiptsDocument
        /// </summary>
        /// <returns></returns>
        public async Task<DataResultDto<List<ResourceDto>>> GetResourcesFilterAsync()
        {
            try
            {
                await using var context = ContextProvider();
                var result = await context.Resources.ToListAsync();

                return DataResultDto<List<ResourceDto>>.CreateFromData(
                    [.. result.Select(v=>new ResourceDto
                    {
                        Id = v.Id,
                        Name = v.Name,
                        Status = v.Status
                    })]
                );
            }
            catch (Exception ex)
            {
                return DataResultDto<List<ResourceDto>>.CreateFromException(ex);
            }
        }

        /// <summary>
        /// Получение Measurements для фильтров ReceiptsDocument
        /// </summary>
        /// <returns></returns>
        public async Task<DataResultDto<List<MeasurementDto>>> GetMeasurementsFilterAsync()
        {
            try
            {
                await using var context = ContextProvider();

                var result = await context.Measurements.ToListAsync();

                return DataResultDto<List<MeasurementDto>>.CreateFromData(
                    [.. result.Select(v=>new MeasurementDto
                    {
                        Id = v.Id,
                        Name = v.Name,
                        Status = v.Status
                    })]
                );
            }
            catch (Exception ex)
            {
                return DataResultDto<List<MeasurementDto>>.CreateFromException(ex);
            }
        }
    }
}
