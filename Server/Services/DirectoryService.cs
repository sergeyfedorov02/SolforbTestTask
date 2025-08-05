using DataContracts;
using Microsoft.EntityFrameworkCore;
using Radzen;
using SolforbTestTask.Client.Pages.Directory;
using SolforbTestTask.Server.Data;
using SolforbTestTask.Server.Extensions;
using SolforbTestTask.Server.Models.Entities;

namespace SolforbTestTask.Server.Services
{
    public class DirectoryService : IDirectoryService
    {
        /// <summary>
        /// // Контекст - для связи объектной модели и БД (напрямую с БД не работают)
        /// </summary>
        private Func<SolforbDBContext> ContextProvider { get; }

        private ILogger<StorageService> Logger { get; }

        public DirectoryService(Func<SolforbDBContext> provider, ILogger<StorageService> logger)
        {
            ContextProvider = provider;
            Logger = logger;
        }

        public async Task<DataResultDto<GridResultDto<ResourceDto>>> GetResourceAsync(Query query, int status)
        {
            try
            {
                await using var context = ContextProvider();

                // проведем фильтрацию по IsArchived 
                var queryable = context.Resources.Where(r => r.Status == status);

                var result = await queryable.GetDataAsync(query, "Id asc");


                return DataResultDto<GridResultDto<ResourceDto>>.CreateFromData(new GridResultDto<ResourceDto>
                {
                    Data = [.. result.Value.Select(v => new ResourceDto
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

        public async Task<ResultDto> CreateResourceAsync(ResourceDto resourceDto)
        {
            try
            {
                await using var context = ContextProvider();

                var resource = new Resource
                {
                    Name = resourceDto.Name.Trim(),
                    Status = 1
                };

                if (context.Resources.FirstOrDefault(r=>r.Name== resource.Name) != null)
                {
                    throw new ArgumentException("Ресурс с таким именем уже существует");
                }

                context.Resources.Add(resource);
                await context.SaveChangesAsync();

                return ResultDto.CreateOk();
            }
            catch(ArgumentException e)
            {
                return ResultDto.CreateFromException(e);
            }
            catch (Exception ex)
            {
                return ResultDto.CreateFromException(new Exception("Ошибка при добавлении в базу", ex));
            }
        }

        public async Task<ResultDto> UpdateResourceAsync(ResourceDto resourceDto)
        {
            try
            {
                await using var context = ContextProvider();

                // проверяем, что такой ресурс есть в Базе данных
                var resource = await context.Resources.FindAsync(resourceDto.Id) ?? throw new ArgumentException("Ресурс не найден");

                // проверяем уникальность нового имени ресурса
                if (context.Resources.Any(r => r.Name == resourceDto.Name && r.Id != resourceDto.Id))
                {
                    throw new ArgumentException("Ресурс с таким именем уже существует");
                }

                resource.Name = resourceDto.Name.Trim();
                await context.SaveChangesAsync();

                return ResultDto.CreateOk();
            }
            catch (ArgumentException e)
            {
                return ResultDto.CreateFromException(e);
            }
            catch (Exception ex)
            {
                return ResultDto.CreateFromException(ex);
            }
        }

        public async Task<ResultDto> ArchiveResourceAsync(ResourceDto resourceDto)
        {
            try
            {
                await using var context = ContextProvider();

                var resource = await context.Resources.FindAsync(resourceDto.Id);
                if (resource == null)
                {
                    throw new ArgumentException("Ресурс не найден");
                }

                // меняем статус
                resource.Status = resource.Status == 1 ? 2 : 1;
                await context.SaveChangesAsync();

                resourceDto.Status = resource.Status;

                return ResultDto.CreateOk();
            }
            catch (ArgumentException e)
            {
                return ResultDto.CreateFromException(e);
            }
            catch (Exception ex)
            {
                return ResultDto.CreateFromException(ex);
            }
        }

        public async Task<ResultDto> DeleteResourceAsync(long resourceId)
        {
            try
            {
                await using var context = ContextProvider();

                // ресурс уже несуществует
                var resource = await context.Resources.FindAsync(resourceId);
                if (resource == null)
                {
                    throw new ArgumentException("Ресурс не найден");
                }

                // ресурс используется где-то -> нельзя удалять
                var errors = new List<string>();

                if (await context.Balances.AnyAsync(b => b.ResourceId == resourceId))
                    errors.Add("используется в Balance");

                if (await context.ReceiptsResources.AnyAsync(r => r.ResourceId == resourceId))
                    errors.Add("используется в ReceiptsResource");

                if (await context.ShipmentsResources.AnyAsync(s => s.ResourceId == resourceId))
                    errors.Add("используется в ShipmentsResource");

                if (errors.Count > 0)
                {
                    throw new InvalidOperationException($"Невозможно удалить ресурс. {string.Join(", ", errors)}");
                }

                // успешное удаление
                context.Resources.Remove(resource);
                await context.SaveChangesAsync();

                return ResultDto.CreateOk();

            }
            catch (InvalidOperationException e)
            {
                return ResultDto.CreateFromException(e);
            }
            catch (Exception ex)
            {
                return ResultDto.CreateFromException(ex);
            }
        }
    }
}
