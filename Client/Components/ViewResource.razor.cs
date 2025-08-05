using DataContracts;
using Microsoft.AspNetCore.Components;
using Radzen;
using SolforbTestTask.Client.Services;

namespace SolforbTestTask.Client.Components
{
    public partial class ViewResource
    {
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected ILogger<CreateResource> Logger { get; set; }

        [Inject]
        protected IDirectoryService DirectoryService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Parameter]
        public ResourceDto ResourceDto { get; set; }

        private ResourceDto editModel = new();

        protected override void OnInitialized()
        {
            editModel = new ResourceDto
            {
                Id = ResourceDto.Id,
                Name = ResourceDto.Name,
                Status = ResourceDto.Status
            };
        }

        private async Task OnSubmit()
        {
            var result = await DirectoryService.UpdateResourceAsync(editModel);

            if (result.Success)
            {
                NotificationService.Notify(NotificationSeverity.Success, "Успешно", "Ресурс обновлен");
                DialogService.Close(editModel);
            }
            else
            {
                Logger.LogError("Ошибка при обновлении ресурса", result.Exception);
                NotificationService.Notify(
                    NotificationSeverity.Error,
                    "Ошибка",
                    result.Exception?.Message ?? "Неизвестная ошибка при обновлении ресурса");
            }
        }

        private async Task OnArchive()
        {
            var result = await DirectoryService.ArchiveResourceAsync(editModel);

            if (result.Success)
            {
                var notifyMessage = editModel.Status == 1 ? "архив" : "рабочие";

                NotificationService.Notify(NotificationSeverity.Success, "Успешно", $"Ресурс перемещен в {notifyMessage}");
                DialogService.Close(editModel);
            }
            else
            {
                Logger.LogError("Ошибка при архивировании ресурса", result.Exception);
                NotificationService.Notify(
                    NotificationSeverity.Error,
                    "Ошибка",
                    result.Exception?.Message ?? "Неизвестная ошибка при архивировании");
            }
        }

        private async Task OnDelete()
        {
            var result = await DirectoryService.DeleteResourceAsync(editModel.Id);

            if (result.Success)
            {
                NotificationService.Notify(NotificationSeverity.Success, "Успешно", "Ресурс удален");
                DialogService.Close(editModel);
            }
            else
            {
                Logger.LogError("Ошибка при удалении ресурса", result.Exception);
                NotificationService.Notify(
                    NotificationSeverity.Error,
                    "Ошибка",
                    result.Exception?.Message ?? "Неизвестная ошибка при удалении");
            }
        }

        private void OnInvalidSubmit()
        {
            NotificationService.Notify(NotificationSeverity.Error, "Ошибка", "Ошибки при заполнении формы");
        }
    }
}
