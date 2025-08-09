using DataContracts;
using Microsoft.AspNetCore.Components;
using Radzen;
using SolforbTestTask.Client.Services;
using System.Collections.Generic;
using System.Resources;

namespace SolforbTestTask.Client.Components
{
    public partial class ViewReceiptDocument
    {
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected ILogger<CreateResource> Logger { get; set; }

        [Inject]
        protected IDirectoryService DirectoryService { get; set; }

        [Inject]
        protected IStorageService StorageService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Parameter]
        public ReceiptDocumentDto ReceiptDocumentDto { get; set; }

        private ReceiptDocumentDto editModel = new();

        private List<ResourceDto> availableResources = new();
        private List<MeasurementDto> availableMeasurements = new();

        protected override async Task OnInitializedAsync()
        {
            editModel = new ReceiptDocumentDto
            {
                Id = ReceiptDocumentDto.Id,
                Number = ReceiptDocumentDto.Number,
                Date = ReceiptDocumentDto.Date,
                ReceiptResources = ReceiptDocumentDto.ReceiptResources
            };

            await LoadResources();
            await LoadMeasurements();
        }

        private async Task LoadResources()
        {
            var result = await DirectoryService.GetResourceAsync(new FilterDirectoryDto
            {
                Status = 1
            });

            if (result.Success)
            {
                // получаем все рабочие Resources на текущий момент (статус = 1)
                availableResources = [.. result.Data.Data];

                // получаем все Resources из архива (статус = 2), которые уже используются в этом документе
                if (editModel.ReceiptResources?.Count > 0)
                {
                    var archivedAvailableResources = editModel.ReceiptResources
                        .Where(r => r.Resource?.Status == 2)
                        .Select(r => r.Resource)
                        .DistinctBy(r => r.Id);  // убираем дубликаты по Id

                    // получаем общие список доступных Resources
                    availableResources.AddRange(archivedAvailableResources);
                }
            }
            else
            {
                //Logger.LogError("Ошибка при добавлении документа поступления", result.Exception);
                //NotificationService.Notify(
                //    NotificationSeverity.Error,
                //    "Ошибка",
                //    result.Exception?.Message ?? "Неизвестная ошибка при получении списка допуступных ресурсов");
            }
        }

        private async Task LoadMeasurements()
        {

            var result = await DirectoryService.GetMeasurementAsync(new FilterDirectoryDto
            {
                Status = 1
            });

            if (result.Success)
            {
                // получаем все рабочие Measurements на текущий момент (статус = 1)
                availableMeasurements = [.. result.Data.Data];

                // получаем все Measurements из архива (статус = 2), которые уже используются в этом документе
                if (editModel.ReceiptResources?.Count > 0)
                {
                    var archivedAvailableMeasurements = editModel.ReceiptResources
                        .Where(r => r.Measurement?.Status == 2)
                        .Select(r => r.Measurement)
                        .DistinctBy(r => r.Id);

                    // получаем общие список доступных Measurements
                    availableMeasurements.AddRange(archivedAvailableMeasurements);
                }
            }
            else
            {
                //Logger.LogError("Ошибка при добавлении документа поступления", result.Exception);
                //NotificationService.Notify(
                //    NotificationSeverity.Error,
                //    "Ошибка",
                //    result.Exception?.Message ?? "Неизвестная ошибка при получении списка допуступных единиц измерения");
            }
        }

        private void AddResource()
        {
            editModel.ReceiptResources.Add(new ReceiptResourceDto
            {
                Resource = new ResourceDto(),
                Measurement = new MeasurementDto(),
                Count = 1
            });
        }

        private void RemoveResource(int index)
        {
            if (editModel.ReceiptResources.Count > 0)
            {
                editModel.ReceiptResources.RemoveAt(index);
            }
        }

        private async Task OnSubmit()
        {
            try
            {
                var ff = 0;

                if (editModel.ReceiptResources.Any(r => r.Resource.Id == 0 || r.Measurement.Id == 0 || r.Count <= 0))
                {
                    NotificationService.Notify(
                            NotificationSeverity.Error,
                            "Ошибка",
                            "Заполните все поля для всех ресурсов");
                    return;
                }

                var itemsToAdd = new HashSet<(long resId, long mesId)>();
                foreach (var item in editModel.ReceiptResources)
                {
                    if (itemsToAdd.Contains((item.Resource.Id, item.Measurement.Id)))
                    {
                        NotificationService.Notify(
                            NotificationSeverity.Error,
                            "Ошибка",
                            "Одинаковые записи: ресурс-единица измерения");
                        return;
                    }
                    itemsToAdd.Add((item.Resource.Id, item.Measurement.Id));
                }


                var result = await StorageService.UpdateReceiptDocumentAsync(editModel);

                if (result.Success)
                {
                    NotificationService.Notify(
                        NotificationSeverity.Success,
                        "Успешно",
                        "Документ поступления обновлен");
                    DialogService.Close(editModel);
                }
                else
                {
                    Logger.LogError("Ошибка при обновлении документа поступления", result.Exception);
                    NotificationService.Notify(
                        NotificationSeverity.Error,
                        "Ошибка",
                        result.Exception?.Message ?? "Неизвестная ошибка при обновлении документа поступления");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Неожиданная ошибка при обновлении документа поступления");
                NotificationService.Notify(
                    NotificationSeverity.Error,
                    "Ошибка",
                    "Произошла непредвиденная ошибка");
            }
        }

        private async Task OnDelete()
        {
            var xx = 0;
            //var result = await DirectoryService.DeleteResourceAsync(editModel.Id);

            //if (result.Success)
            //{
            //    NotificationService.Notify(NotificationSeverity.Success, "Успешно", "Ресурс удален");
            //    DialogService.Close(editModel);
            //}
            //else
            //{
            //    Logger.LogError("Ошибка при удалении ресурса", result.Exception);
            //    NotificationService.Notify(
            //        NotificationSeverity.Error,
            //        "Ошибка",
            //        result.Exception?.Message ?? "Неизвестная ошибка при удалении ресурса");
            //}
        }

        private void OnInvalidSubmit()
        {
            NotificationService.Notify(NotificationSeverity.Error, "Ошибка", "Ошибки при заполнении формы");
        }
    }
}
