using DataContracts;

namespace SolforbTestTask.Client.Services
{
    public interface IDirectoryService
    {
        Task<DataResultDto<GridResultDto<ResourceDto>>> GetResourceAsync(FilterDirectoryDto filterDirectoryDto);

        Task<ResultDto> CreateResourceAsync(ResourceDto resourceDto);

        Task<ResultDto> UpdateResourceAsync(ResourceDto resourceDto);

        Task<ResultDto> ArchiveResourceAsync(ResourceDto resourceDto);

        Task<ResultDto> DeleteResourceAsync(long resourceId);
    }
}
