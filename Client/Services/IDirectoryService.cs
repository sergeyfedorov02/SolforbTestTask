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

        Task<DataResultDto<GridResultDto<MeasurementDto>>> GetMeasurementAsync(FilterDirectoryDto filterDirectoryDto);

        Task<ResultDto> CreateMeasurementAsync(MeasurementDto measurementDto);

        Task<ResultDto> UpdateMeasurementAsync(MeasurementDto measurementDto);

        Task<ResultDto> ArchiveMeasurementAsync(MeasurementDto measurementDto);

        Task<ResultDto> DeleteMeasurementAsync(long measurementId);
    }
}
