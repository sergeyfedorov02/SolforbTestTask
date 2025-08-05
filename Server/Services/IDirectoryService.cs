using Radzen;
using DataContracts;

namespace SolforbTestTask.Server.Services
{
    public interface IDirectoryService
    {
        Task<DataResultDto<GridResultDto<ResourceDto>>> GetResourceAsync(Query query, int status);

        Task<ResultDto> CreateResourceAsync(ResourceDto resourceDto);

        Task<ResultDto> UpdateResourceAsync(ResourceDto resourceDto);

        Task<ResultDto> ArchiveResourceAsync(ResourceDto resourceDto);

        Task<ResultDto> DeleteResourceAsync(long resourceId);
    }
}
