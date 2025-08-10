# SolforbTestTask

## Как собрать и запустить приложение:

1. Клонировать репозиторий

```git clone https://github.com/sergeyfedorov02/SolforbTestTask.git```

2. Перейти в рабочий каталог

```cd SolforbTestTask```

3. Сделать публикацию для работы web-сервера

```dotnet publish Server/SolforbTestTask.Server.csproj -p:PublishProfile=FolderProfile```

4. Перейти в каталог с исполняемым файлом и запустить его

```
cd Server\bin\Release\net9.0\publish
SolforbTestTask.Server.exe
```

Приложение будет запущено по адресу *http://localhost:5000/*


## Информация о проекте:
1. Для формирования начального проекта использовался Radzen Blazor Studio

*https://www.radzen.com/blazor-studio/*

2. Приложение является blazor-WebAssembly-приложением
3. Предполагается, что локально развернут сервер PostgreSQL. Развернуть можно в Docker,
   используя следующие команды

```
docker pull postgres
docker run -itd -e POSTGRES_USER=admin -e POSTGRES_PASSWORD=admin -p 5432:5432 --name postgresql postgres
```
Имя пользователя и пароль: admin/admin (Такие параметры указаны в строке подключения приложения)

4. Для доступа к базе данных используется Entity Framework и провайдер PostgreSQL. 
При запуске приложения создается база с именем *solforbDb*, если база с таким именем уже есть, 
то используется она.

## Демонстрация работы

Основная страница с отображением Баланса Склада

![Balance page](Images/balance_page.PNG)

На данной странице настроена фильтрация

![Balance page filter 1](Images/balance_page_filter_1.PNG)

Присутствует возможность множественного выбора

![Balance page filter 2](Images/balance_page_filter_2.PNG)

Далее рассмотрим страницу "Ресурсы"

![Resource page work](Images/resource_page_work.PNG)

Попробуем добавить новый ресурс
![Resource page add](Images/resource_page_add.PNG)

Как мы видим, ресурс успешно добавлен
![Resource page add success](Images/resource_page_add_success.PNG)

Попробуем добавить еще один ресурс, но появляется ошибка
![Resource page add fail](Images/resource_page_add_fail.PNG)

Дело в том, что ресурс уже создан, просто находится в архиве

![Resource page archived](Images/resource_page_archived.PNG)

Также все записи можно редактировать при двойном нажатии на них.

![Resource view work](Images/resource_view_work.PNG)

Следует обратить внимание, что записи из рабочего пространства и из архива имеют различающиеся кнопки для перемещения
![Resource view archived](Images/resource_view_archived.PNG)

Аналогичная страница "Единицы измерения"
![Measurement page work](Images/measurement_page_work.PNG)

Рассмотрим теперь страницу "Поступления"

![Receipt page](Images/receipt_page.PNG)

Здесь также применимы фильтры с множественным выбором

![Receipt filter page](Images/receipt_filter_page.PNG)

Далее приведена стандартная форма добавления нового "документа поступления"

![Receipt add 1](Images/receipt_add_1.PNG)

При нажатии на кнопку "Добавить ресурс", появляется форма для заполнения, которую можно удалить 

![Receipt add 2](Images/receipt_add_2.PNG)

При попытке сохранить документ с незаполненными полями, высвевится соответствующая ошибка

![Receipt add fail 1](Images/receipt_add_fail_1.PNG)

Добавим новый "документ поступления"

![Receipt add fail 1](Images/receipt_add_success_1.PNG)

Как мы видим, он был успешно добавлен и появился в списке

![Receipt add fail 2](Images/receipt_add_success_2.PNG)

Также можно редактировать документы поступления при двойном нажатии

![Receipt view page](Images/receipt_view_page.PNG)

Отредактируем созданную ранее запись

![Receipt view page create](Images/receipt_view_page_create.PNG)

Посмотрим на изменение "Баланса" для всего склада

![Balance page after update receipt](Images/balance_page_after_update_receipt.PNG)
