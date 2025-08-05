using DataContracts;
using Microsoft.AspNetCore.Components;
using Radzen;
using SolforbTestTask.Client.Services;

namespace SolforbTestTask.Client.Components
{
    public partial class ViewMeasurement
    {
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected ILogger<CreateMeasurement> Logger { get; set; }

        [Inject]
        protected IDirectoryService DirectoryService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Parameter]
        public MeasurementDto MeasurementDto { get; set; }

        private MeasurementDto editModel = new();

        protected override void OnInitialized()
        {
            editModel = new MeasurementDto
            {
                Id = MeasurementDto.Id,
                Name = MeasurementDto.Name,
                Status = MeasurementDto.Status
            };
        }

        private async Task OnSubmit()
        {
            var result = await DirectoryService.UpdateMeasurementAsync(editModel);

            if (result.Success)
            {
                NotificationService.Notify(NotificationSeverity.Success, "Успешно", "Единица измерения обновлена");
                DialogService.Close(editModel);
            }
            else
            {
                Logger.LogError("Ошибка при обновлении единицы измерения", result.Exception);
                NotificationService.Notify(
                    NotificationSeverity.Error,
                    "Ошибка",
                    result.Exception?.Message ?? "Неизвестная ошибка при обновлении единицы измерения");
            }
        }

        private async Task OnArchive()
        {
            var result = await DirectoryService.ArchiveMeasurementAsync(editModel);

            if (result.Success)
            {
                var notifyMessage = editModel.Status == 1 ? "архив" : "рабочие";

                NotificationService.Notify(NotificationSeverity.Success, "Успешно", $"Единица измерения перемещена в {notifyMessage}");
                DialogService.Close(editModel);
            }
            else
            {
                Logger.LogError("Ошибка при архивировании единицы измерения", result.Exception);
                NotificationService.Notify(
                    NotificationSeverity.Error,
                    "Ошибка",
                    result.Exception?.Message ?? "Неизвестная ошибка при архивировании");
            }
        }

        private async Task OnDelete()
        {
            var result = await DirectoryService.DeleteMeasurementAsync(editModel.Id);

            if (result.Success)
            {
                NotificationService.Notify(NotificationSeverity.Success, "Успешно", "Единица измерения удалена");
                DialogService.Close(editModel);
            }
            else
            {
                Logger.LogError("Ошибка при удалении единицы измерения", result.Exception);
                NotificationService.Notify(
                    NotificationSeverity.Error,
                    "Ошибка",
                    result.Exception?.Message ?? "Неизвестная ошибка при удалении единицы измерения");
            }
        }

        private void OnInvalidSubmit()
        {
            NotificationService.Notify(NotificationSeverity.Error, "Ошибка", "Ошибки при заполнении формы");
        }
    }
}
