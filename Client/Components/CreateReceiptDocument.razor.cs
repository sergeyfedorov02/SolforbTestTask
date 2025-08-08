using DataContracts;
using Microsoft.AspNetCore.Components;
using Radzen;
using SolforbTestTask.Client.Services;

namespace SolforbTestTask.Client.Components
{
    public partial class CreateReceiptDocument
    {
        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected ILogger<CreateReceiptDocument> Logger { get; set; }

        [Inject]
        protected IStorageService StorageService { get; set; }

        [Inject]
        protected IDirectoryService DirectoryService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        private ReceiptDocumentDto model = new ReceiptDocumentDto
        {
            Date = DateOnly.FromDateTime(DateTime.Now),
            ReceiptResources = new List<ReceiptResourceDto> ()
            
        };

        private List<ResourceDto> availableResources = new();
        private List<MeasurementDto> availableMeasurements = new();

        protected override async Task OnInitializedAsync()
        {
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
                availableResources = [.. result.Data.Data];
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
                availableMeasurements = [.. result.Data.Data];
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
            model.ReceiptResources.Add(new ReceiptResourceDto
            {
                Resource = new ResourceDto(),
                Measurement = new MeasurementDto(),
                Count = 1
            });
        }

        private void RemoveResource(int index)
        {
            if (model.ReceiptResources.Count > 0)
            {
                model.ReceiptResources.RemoveAt(index);
            }
        }

        private async Task OnSubmit()
        {
            try
            {
                
                if (model.ReceiptResources.Any(r => r.Resource.Id == 0 || r.Measurement.Id == 0 || r.Count <= 0))
                {
                    NotificationService.Notify(
                            NotificationSeverity.Error,
                            "Ошибка",
                            "Заполните все поля для всех ресурсов");
                    return;
                }

                var receiptDocumentDto = new ReceiptDocumentDto
                {
                    Date = model.Date,
                    Number = model.Number,
                    ReceiptResources = [.. model.ReceiptResources.Select(r => new ReceiptResourceDto
                    {
                        Resource = r.Resource,
                        Measurement = r.Measurement,
                        Count = r.Count
                    })]
                };

                var itemsToAdd = new HashSet<(long resId, long mesId)>();
                foreach(var item in receiptDocumentDto.ReceiptResources)
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


                var result = await StorageService.CreateReceiptDocumentAsync(receiptDocumentDto);

                if (result.Success)
                {
                    NotificationService.Notify(
                        NotificationSeverity.Success,
                        "Успешно",
                        "Документ поступления добавлен");
                    DialogService.Close(receiptDocumentDto);
                }
                else
                {
                    Logger.LogError("Ошибка при создании документа поступления", result.Exception);
                    NotificationService.Notify(
                        NotificationSeverity.Error,
                        "Ошибка",
                        result.Exception?.Message ?? "Неизвестная ошибка при создании документа поступления");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Неожиданная ошибка при создании документа поступления");
                NotificationService.Notify(
                    NotificationSeverity.Error,
                    "Ошибка",
                    "Произошла непредвиденная ошибка");
            }
        }

        private void OnInvalidSubmit()
        {
            NotificationService.Notify(NotificationSeverity.Error, "Ошибка", "Ошибки при заполнении формы");
        }
    }
}
