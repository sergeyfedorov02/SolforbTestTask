# SolforbTestTask

## ��� ������� � ��������� ����������:

1. ����������� �����������

```git clone https://github.com/sergeyfedorov02/SolforbTestTask.git```

2. ������� � ������� �������

```cd SolforbTestTask```

3. ������� ���������� ��� ������ web-�������

```dotnet publish Server/SolforbTestTask.Server.csproj -p:PublishProfile=FolderProfile```

4. ������� � ������� � ����������� ������ � ��������� ���

```
cd Server\bin\Release\net9.0\publish
SolforbTestTask.Server.exe
```

���������� ����� �������� �� ������ *http://localhost:5000/*


## ���������� � �������:
1. ��� ������������ ���������� ������� ������������� Radzen Blazor Studio

*https://www.radzen.com/blazor-studio/*

2. ���������� �������� blazor-WebAssembly-�����������
3. ��������������, ��� �������� ��������� ������ PostgreSQL. ���������� ����� � Docker,
   ��������� ��������� �������

```
docker pull postgres
docker run -itd -e POSTGRES_USER=admin -e POSTGRES_PASSWORD=admin -p 5432:5432 --name postgresql postgres
```
��� ������������ � ������: admin/admin (����� ��������� ������� � ������ ����������� ����������)

4. ��� ������� � ���� ������ ������������ Entity Framework � ��������� PostgreSQL. 
��� ������� ���������� ��������� ���� � ������ *solforbDb*, ���� ���� � ����� ������ ��� ����, 
�� ������������ ���.

## ������������ ������

�������� �������� � ������������ ������� ������

![Balance page](Images/balance_page.PNG)

�� ������ �������� ��������� ����������

![Balance page filter 1](Images/balance_page_filter_1.PNG)

������������ ����������� �������������� ������

![Balance page filter 2](Images/balance_page_filter_2.PNG)

����� ���������� �������� "�������"

![Resource page work](Images/resource_page_work.PNG)

��������� �������� ����� ������
![Resource page add](Images/resource_page_add.PNG)

��� �� �����, ������ ������� ��������
![Resource page add success](Images/resource_page_add_success.PNG)

��������� �������� ��� ���� ������, �� ���������� ������
![Resource page add fail](Images/resource_page_add_fail.PNG)

���� � ���, ��� ������ ��� ������, ������ ��������� � ������

![Resource page archived](Images/resource_page_archived.PNG)

����� ��� ������ ����� ������������� ��� ������� ������� �� ���.

![Resource view work](Images/resource_view_work.PNG)

������� �������� ��������, ��� ������ �� �������� ������������ � �� ������ ����� ������������� ������ ��� �����������
![Resource view archived](Images/resource_view_archived.PNG)

����������� �������� "������� ���������"
![Measurement page work](Images/measurement_page_work.PNG)

���������� ������ �������� "�����������"

![Receipt page](Images/receipt_page.PNG)

����� ����� ��������� ������� � ������������� �������

![Receipt filter page](Images/receipt_filter_page.PNG)

����� ��������� ����������� ����� ���������� ������ "��������� �����������"

![Receipt add 1](Images/receipt_add_1.PNG)

��� ������� �� ������ "�������� ������", ���������� ����� ��� ����������, ������� ����� ������� 

![Receipt add 2](Images/receipt_add_2.PNG)

��� ������� ��������� �������� � �������������� ������, ���������� ��������������� ������

![Receipt add fail 1](Images/receipt_add_fail_1.PNG)

������� ����� "�������� �����������"

![Receipt add fail 1](Images/receipt_add_success_1.PNG)

��� �� �����, �� ��� ������� �������� � �������� � ������

![Receipt add fail 2](Images/receipt_add_success_2.PNG)

����� ����� ������������� ��������� ����������� ��� ������� �������

![Receipt view page](Images/receipt_view_page.PNG)

������������� ��������� ����� ������

![Receipt view page create](Images/receipt_view_page_create.PNG)

��������� �� ��������� "�������" ��� ����� ������

![Balance page after update receipt](Images/balance_page_after_update_receipt.PNG)
