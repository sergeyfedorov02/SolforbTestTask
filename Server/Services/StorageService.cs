using DataContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
                        ReceiptItem = new ReceiptResourceDto
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
                        ReceiptItem = new ReceiptResourceDto
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
                    queryAll = queryAll.Where(d => query.ResourceIds.Contains(d.ReceiptItem.Resource.Id));
                }

                if (query.MeasurementIds != null && query.MeasurementIds.Count != 0)
                {
                    queryAll = queryAll.Where(d => query.MeasurementIds.Contains(d.ReceiptItem.Measurement.Id));
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

        /// <summary>
        /// Валидация ресурсов и единиц измерения
        /// </summary>
        /// <param name="context"></param>
        /// <param name="receiptDocumentDto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private async Task ValidateReceiptDocument(SolforbDBContext context, ReceiptDocumentDto receiptDocumentDto)
        {
            var receiptResources = receiptDocumentDto.ReceiptResources ?? [];

            if (receiptResources.Count != 0)
            {
                // для ресурсов
                var resourceIds = new HashSet<long>(receiptResources.Select(r => r.Resource.Id));

                var existingResources = new HashSet<long>(await context.Resources.Where(r => resourceIds.Contains(r.Id) && r.Status == 1)
                    .Select(r => r.Id)
                    .ToListAsync());

                resourceIds.ExceptWith(existingResources);

                if (resourceIds.Count > 0)
                {
                    throw new ArgumentException($"Ресурсы с Id = {string.Join(',', resourceIds)} не найдены");
                }


                // для единиц измерения
                var measurementIds = new HashSet<long>(receiptResources.Select(r => r.Measurement.Id));

                var existingMeasurements = new HashSet<long>(await context.Measurements.Where(m => measurementIds.Contains(m.Id) && m.Status == 1)
                    .Select(m => m.Id)
                    .ToListAsync());

                measurementIds.ExceptWith(existingMeasurements);

                if (measurementIds.Count > 0)
                {
                    throw new ArgumentException($"Единицы измерения с Id = {string.Join(',', measurementIds)} не найдены");
                }
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

                    // проверка ресурсов и единиц измерения для ReceiptResource
                    await ValidateReceiptDocument(context, receiptDocumentDto);
                    
                    // ReceiptResource
                    foreach (var receiptResourceDto in receiptDocumentDto.ReceiptResources ?? [])
                    {
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
        public async Task<DataResultDto<GridResultDto<string>>> GetReceiptsDocumentNumbersFilterAsync(Query query)
        {
            try
            {
                await using var context = ContextProvider();

                var result = await context.ReceiptsDocuments.GetDataAsync(query, "Id asc");               

                return DataResultDto<GridResultDto<string>>.CreateFromData(new GridResultDto<string>
                {
                    Data = [.. result.Value.Select(d => d.Number)],
                    Count = result.Count
                });
            }
            catch (Exception ex)
            {
                return DataResultDto<GridResultDto<string>>.CreateFromException(ex);
            }
        }

        /// <summary>
        /// Получение Resources для фильтров ReceiptsDocument
        /// </summary>
        /// <returns></returns>
        public async Task<DataResultDto<GridResultDto<ResourceDto>>> GetResourcesFilterAsync(Query query)
        {
            try
            {
                await using var context = ContextProvider();
                var result = await context.Resources.GetDataAsync(query, "Id asc");

                return DataResultDto<GridResultDto<ResourceDto>>.CreateFromData(new GridResultDto<ResourceDto>
                {
                    Data = [.. result.Value.Select(v=>new ResourceDto
                    {
                        Id = v.Id,
                        Name = v.Name,
                        Status = v.Status
                    })],
                    Count = result.Count
                });
            }
            catch (Exception ex)
            {
                return DataResultDto<GridResultDto<ResourceDto>>.CreateFromException(ex);
            }
        }

        /// <summary>
        /// Получение Measurements для фильтров ReceiptsDocument
        /// </summary>
        /// <returns></returns>
        public async Task<DataResultDto<GridResultDto<MeasurementDto>>> GetMeasurementsFilterAsync(Query query)
        {
            try
            {
                await using var context = ContextProvider();

                var result = await context.Measurements.GetDataAsync(query, "Id asc");

                return DataResultDto<GridResultDto<MeasurementDto>>.CreateFromData(new GridResultDto<MeasurementDto>
                {
                    Data = [.. result.Value.Select(v=>new MeasurementDto
                    {
                        Id = v.Id,
                        Name = v.Name,
                        Status = v.Status
                    })],
                    Count = result.Count
                });
            }
            catch (Exception ex)
            {
                return DataResultDto<GridResultDto<MeasurementDto>>.CreateFromException(ex);
            }
        }

        /// <summary>
        /// Получение ReceiptDocument из заданного ReceiptDocumentItemDto
        /// </summary>
        /// <param name="receiptDocumentItemDto"></param>
        /// <returns></returns>
        public async Task<DataResultDto<ReceiptDocumentDto>> GetWholeReceiptDocumentAsync(ReceiptDocumentItemDto receiptDocumentItemDto)
        {
            try
            {
                await using var context = ContextProvider();

                var result = await context.ReceiptsResources
                    .Include(r => r.Resource)
                    .Include(r => r.Measurement)
                    .Where(r => r.DocumentId == receiptDocumentItemDto.Id)
                    .ToListAsync();

                var receiptResources = result.Count == 0 ? [] : result.Select(r => new ReceiptResourceDto
                {
                    Resource = new ResourceDto
                    {
                        Id = r.ResourceId,
                        Name = r.Resource.Name ,
                        Status = r.Resource.Status
                    },
                    Measurement = new MeasurementDto
                    {
                        Id = r.MeasurementId,
                        Name = r.Measurement.Name,
                        Status = r.Measurement.Status
                    },
                    Count = r.Count
                }).ToList();

                return DataResultDto<ReceiptDocumentDto>.CreateFromData(new ReceiptDocumentDto
                {
                    Id = receiptDocumentItemDto.Id,
                    Number = receiptDocumentItemDto.Number,
                    Date = receiptDocumentItemDto.Date,
                    ReceiptResources = receiptResources
                });
            }
            catch (Exception ex)
            {
                return DataResultDto<ReceiptDocumentDto>.CreateFromException(ex);
            }
        }

        /// <summary>
        /// Обновление Balance при UpdateReceiptDocument
        /// </summary>
        /// <param name="context"></param>
        /// <param name="resourceId"></param>
        /// <param name="measurementId"></param>
        /// <param name="countDiff"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private async Task UpdateBalance(SolforbDBContext context, long resourceId, long measurementId, int countDiff)
        {
            if (countDiff == 0) return;

            var balance = await context.Balances.FirstOrDefaultAsync(b => b.ResourceId == resourceId && b.MeasurementId == measurementId);

            if (balance != null)
            {
                balance.Count += countDiff;
                if (balance.Count < 0)
                {
                    throw new InvalidOperationException("Баланс не может быть отрицательным");
                }

                // если count == 0 -> удаляем эту запись
                if (balance.Count == 0)
                {
                    context.Balances.Remove(balance);
                }
                else
                {
                    context.Balances.Update(balance);
                }
            }
            else if (countDiff > 0)
            {
                context.Balances.Add(new Balance
                {
                    ResourceId = resourceId,
                    MeasurementId = measurementId,
                    Count = countDiff
                });
            }
            else
            {
                throw new InvalidOperationException("Попытка уменьшить несуществующий баланс");
            }
        }

        /// <summary>
        /// Обновлении ReceiptResources при UpdateReceiptDocument
        /// </summary>
        /// <param name="context"></param>
        /// <param name="existingDocument"></param>
        /// <param name="receiptDocumentDto"></param>
        /// <returns></returns>
        private async Task ProcessReceiptResourcesUpdate(SolforbDBContext context, ReceiptsDocument existingDocument, ReceiptDocumentDto receiptDocumentDto)
        {
            // Введем словарь существующих ресурсов для быстрого поиска
            var existingResourcesDict = existingDocument.ReceiptsResources
                .ToDictionary(r => (r.ResourceId, r.MeasurementId));

            // ReceiptResource обновление
            foreach (var resourceDto in receiptDocumentDto.ReceiptResources ?? [])
            {
                var key = (resourceDto.Resource.Id, resourceDto.Measurement.Id);

                // Ресурс существует -> обновляем количество
                if (existingResourcesDict.TryGetValue(key, out var existingResource))
                {
                    // обновляем
                    var countDiff = resourceDto.Count - existingResource.Count;
                    existingResource.Count = resourceDto.Count;
                    context.ReceiptsResources.Update(existingResource);

                    // Обновляем Balance
                    await UpdateBalance(context, key.Item1, key.Item2, countDiff);
                }
                else
                {
                    // Новый ресурс -> добавляем
                    var newResource = new ReceiptsResource
                    {
                        DocumentId = existingDocument.Id,
                        ResourceId = resourceDto.Resource.Id,
                        MeasurementId = resourceDto.Measurement.Id,
                        Count = resourceDto.Count
                    };
                    context.ReceiptsResources.Add(newResource);

                    // Обновляем Balance
                    await UpdateBalance(context, key.Item1, key.Item2, resourceDto.Count);
                }
            }

            // Удаляем ресурсы, которых больше нет в DTO
            var dtoResourceKeys = new HashSet<(long, long)>(
                receiptDocumentDto.ReceiptResources?.Select(r => (r.Resource.Id, r.Measurement.Id)) ?? []);

            foreach (var existingResource in existingDocument.ReceiptsResources.ToList())
            {
                var key = (existingResource.ResourceId, existingResource.MeasurementId);
                if (!dtoResourceKeys.Contains(key))
                {
                    // Удаляем ресурс и корректируем баланс
                    context.ReceiptsResources.Remove(existingResource);
                    await UpdateBalance(context, key.ResourceId, key.MeasurementId, -existingResource.Count);
                }
            }
        }

        /// <summary>
        /// Валидация ресурсов и единиц измерения при Update ReceiptDocument
        /// </summary>
        /// <param name="context"></param>
        /// <param name="receiptDocumentDto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private async Task ValidateReceiptDocumentUpdate(SolforbDBContext context, ReceiptsDocument existingDocument, ReceiptDocumentDto receiptDocumentDto)
        {
            var receiptResources = receiptDocumentDto.ReceiptResources ?? [];
            if (receiptResources.Count == 0) return;

            // Введем hashSet для присутсвоваших ResourceId и MeasurementId в документе
            var existingResourceIds = existingDocument.ReceiptsResources.Select(r => r.ResourceId).ToHashSet();
            var existingMeasurementIds = existingDocument.ReceiptsResources.Select(r => r.MeasurementId).ToHashSet();

            //var existingResources = existingDocument.ReceiptsResources.Select(r=>r.Resource)

            // все текущие Id для Resource и Measurement из receiptDocumentDto.receiptResources
            var resourceIds = receiptResources.Select(r => r.Resource.Id).ToList();
            var measurementIds = receiptResources.Select(r => r.Measurement.Id).ToList();

            // записи из базы данных 
            var dbResources = await context.Resources
                .Where(r => resourceIds.Contains(r.Id))
                .ToDictionaryAsync(r => r.Id);

            var dbMeasurements = await context.Measurements
                .Where(m => measurementIds.Contains(m.Id))
                .ToDictionaryAsync(m => m.Id);

            var errors = new List<string>();

            // валидация
            foreach (var receiptResource in receiptResources)
            {
                // проверяем текущие Id на предмет существования в базе данных
                var resourceId = receiptResource.Resource.Id;
                var measurementId = receiptResource.Measurement.Id;

                if (!dbResources.ContainsKey(resourceId))
                {
                    errors.Add($"Ресурс с Id = {resourceId} не найден в базе");
                    continue;
                }

                if (!dbMeasurements.ContainsKey(measurementId))
                {
                    errors.Add($"Единица измерения с Id = {measurementId} не найдена в базе");
                    continue;
                }

                // получаем записи о текущем Resource и Measurement из базы данных по Id
                var dbResource = dbResources[resourceId];
                var dbMeasurement = dbMeasurements[measurementId];

                // валидация Status для Resource
                if (existingResourceIds.Contains(resourceId))
                {
                    // для существующих ->  Status должен совпадать с тем, что был в документе для этого же по Id
                    var existingDocumentResourceStatus = existingDocument.ReceiptsResources
                        .Where(r => r.ResourceId == resourceId)
                        .Select(r => r.Resource.Status)
                        .FirstOrDefault();

                    if (dbResource.Status != existingDocumentResourceStatus)
                    {
                        errors.Add($"Статус ресурса {resourceId} не совпадает (в документе: {existingDocumentResourceStatus}, в базе: {dbResource.Status})");
                    }
                }
                else
                {
                    // для новых -> Status должен быть равен 1
                    if (dbResource.Status != 1)
                    {
                        errors.Add($"Новый ресурс {resourceId} имеет статус {dbResource.Status} (должен быть 1)");
                    }
                }

                // валидация Status для Measurement
                if (existingMeasurementIds.Contains(measurementId))
                {
                    // для существующих ->  Status должен совпадать с тем, что был в документе для этого же по Id
                    var existingDocumentMeasurementStatus = existingDocument.ReceiptsResources
                        .Where(r => r.MeasurementId == measurementId)
                        .Select(r => r.Measurement.Status)
                        .FirstOrDefault();

                    if (dbMeasurement.Status != existingDocumentMeasurementStatus)
                    {
                        errors.Add($"Статус единицы измерения {measurementId} не совпадает (в документе: {existingDocumentMeasurementStatus}, в базе: {dbMeasurement.Status})");
                    }
                }
                else
                {
                    // для новых -> Status должен быть равен 1
                    if (dbMeasurement.Status != 1)
                    {
                        errors.Add($"Новая единица измерения {measurementId} имеет статус {dbMeasurement.Status} (должен быть 1)");
                    }
                }
            }

            // обработка накопленных ошибок
            if (errors.Count > 0)
            {
                throw new ArgumentException(string.Join("\n", errors));
            }
        }

        public async Task<ResultDto> UpdateReceiptDocumentAsync(ReceiptDocumentDto receiptDocumentDto)
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
                    // проверяем, что такой ReceiptDocument есть в базе данных
                    var existingDocument = await context.ReceiptsDocuments
                        .Include(d => d.ReceiptsResources)
                        .FirstOrDefaultAsync(d => d.Id == receiptDocumentDto.Id) ?? throw new ArgumentException("Документ поступления не найден");

                    // проверяем уникальность Number, если изменился 
                    if (existingDocument.Number != receiptDocumentDto.Number && 
                        await context.ReceiptsDocuments.AnyAsync(r => r.Number == receiptDocumentDto.Number))
                    {
                        throw new ArgumentException("Документ поступления с таким номером уже существует");
                    }

                    // Обновляем поля Date и Number 
                    existingDocument.Number = receiptDocumentDto.Number;
                    existingDocument.Date = receiptDocumentDto.Date;

                    // Проверка ресурсов и единиц измерения
                    await ValidateReceiptDocumentUpdate(context, existingDocument, receiptDocumentDto);

                    // Обработка изменений в ресурсах (ReceiptResource)
                    await ProcessReceiptResourcesUpdate(context, existingDocument, receiptDocumentDto);

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
                        Logger.LogWarning(conEx, $"Не удалось обновить документ поступления после {maxRetryAttempts} попыток");
                        return ResultDto.CreateFromException(new Exception("Не удалось обновить документ из-за конфликта параллельных изменений"));
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
                    Logger.LogError(ex, "Ошибка при обновлении документа поступления");
                    return ResultDto.CreateFromException(new Exception("Ошибка при обновлении документа поступления", ex));
                }
            }

            return ResultDto.CreateFromException(new Exception("Неизвестная ошибка при обновлении документа поступления"));
        }
    }
}
