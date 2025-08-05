using DataContracts;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SolforbTestTask.Client.Services
{
    public class DirectoryService : IDirectoryService
    {
        private readonly HttpClient _httpClient;

        public DirectoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Получение списка всех Resource
        /// </summary>
        /// <param name="filterDirectoryDto"></param>
        /// <returns></returns>
        public async Task<DataResultDto<GridResultDto<ResourceDto>>> GetResourceAsync(FilterDirectoryDto filterDirectoryDto)
        {
            try
            {
                var x = await _httpClient.PostAsJsonAsync($"api/directory/getResource?", filterDirectoryDto);

                if (!x.IsSuccessStatusCode)
                {
                    throw new Exception($"Ошибка при получении данных для Resource от Сервера {x.StatusCode}");
                }

                var data = await x.Content.ReadFromJsonAsync<GridResultDto<ResourceDto>>();
                return DataResultDto<GridResultDto<ResourceDto>>.CreateFromData(data);
            }
            catch (Exception ex)
            {
                return DataResultDto<GridResultDto<ResourceDto>>.CreateFromException(ex);
            }
        }

        /// <summary>
        /// Создание Resource 
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        public async Task<ResultDto> CreateResourceAsync(ResourceDto resourceDto)
        {
            try
            {
                var x = await _httpClient.PostAsJsonAsync("api/directory/createResource", resourceDto);

                if (!x.IsSuccessStatusCode)
                {
                    return ResultDto.CreateFromException(new Exception($"Ошибка при создании Resource от Сервера статус={x.StatusCode}"));
                }
                return ResultDto.CreateOk();
            }

            catch (Exception ex)
            {
                return ResultDto.CreateFromException(new Exception("Ошибка при создании Resource от Сервера", ex));
            }
        }

        public async Task<ResultDto> UpdateResourceAsync(ResourceDto resourceDto)
        {
            try
            {
                var x = await _httpClient.PutAsJsonAsync("api/directory/updateResource", resourceDto);

                if (!x.IsSuccessStatusCode)
                {
                    return ResultDto.CreateFromException(new Exception($"Ошибка при редактировании Resource от Сервера статус={x.StatusCode}"));
                }
                return ResultDto.CreateOk();
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
                var x = await _httpClient.PostAsJsonAsync("api/directory/archiveResource", resourceDto);

                if (!x.IsSuccessStatusCode)
                {
                    return ResultDto.CreateFromException(new Exception($"Ошибка при архивировании Resource от Сервера статус={x.StatusCode}"));
                }

                return ResultDto.CreateOk();
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
                var x = await _httpClient.DeleteAsync($"api/directory/deleteResource/{resourceId}");

                if (!x.IsSuccessStatusCode)
                {
                    return ResultDto.CreateFromException(new Exception($"Ошибка при удалении Resource от Сервера статус={x.StatusCode}"));
                }

                return ResultDto.CreateOk();
            }
            catch (Exception ex)
            {
                return ResultDto.CreateFromException(ex);
            }
        }
    }
}
