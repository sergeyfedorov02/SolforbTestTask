using DataContracts;
using Microsoft.AspNetCore.Components;
using Radzen;
using SolforbTestTask.Client.Services;

namespace SolforbTestTask.Client.Components
{
    public partial class CreateResource
    {
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected ILogger<CreateResource> Logger { get; set; }

        [Inject]
        protected IDirectoryService DirectoryService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        private ResourceDto model = new();

        private async Task OnSubmit()
        {                        
            var result = await DirectoryService.CreateResourceAsync(model.Name);

            if (result.Success)
            {
                NotificationService.Notify(NotificationSeverity.Success, "Успешно", "Ресурс добавлен");
                DialogService.Close(model);
            }
            else
            {
                Logger.LogError("Ошибка при добавлении ресурса", result.Exception);
                NotificationService.Notify(
                    NotificationSeverity.Error,
                    "Безуспешно",
                    result.Exception?.Message ?? "Неизвестная ошибка при добавлении ресурса");
            }           
        }

        private void OnInvalidSubmit()
        {
            NotificationService.Notify(NotificationSeverity.Error, "Ошибка", "Ошибки при заполнении формы");
        }
    }
}
