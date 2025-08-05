using DataContracts;
using Microsoft.AspNetCore.Components;
using Radzen;
using SolforbTestTask.Client.Services;

namespace SolforbTestTask.Client.Components
{
    public partial class CreateMeasurement
    {
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected ILogger<CreateMeasurement> Logger { get; set; }

        [Inject]
        protected IDirectoryService DirectoryService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        private MeasurementDto model = new();

        private async Task OnSubmit()
        {
            var result = await DirectoryService.CreateMeasurementAsync(model);

            if (result.Success)
            {
                NotificationService.Notify(NotificationSeverity.Success, "Успешно", "Единица измерения добавлена");
                DialogService.Close(model);
            }
            else
            {
                Logger.LogError("Ошибка при добавлении единицы измерения", result.Exception);
                NotificationService.Notify(
                    NotificationSeverity.Error,
                    "Безуспешно",
                    result.Exception?.Message ?? "Неизвестная ошибка при добавлении единицы измерения");
            }
        }

        private void OnInvalidSubmit()
        {
            NotificationService.Notify(NotificationSeverity.Error, "Ошибка", "Ошибки при заполнении формы");
        }
    }
}
