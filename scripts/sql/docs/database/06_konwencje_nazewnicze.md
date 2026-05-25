# Konwencje nazewnicze

## 1. Klucze główne

```text
NazwaTabeliId
```

Przykłady:

```text
PatientId
DepartmentId
MedicalRecordId
ApplicationUserId
```

## 2. Klucze obce

Klucz obcy ma taką samą nazwę jak klucz główny tabeli nadrzędnej.

Przykłady:

```text
DepartmentId
PatientId
ApplicationUserId
```

## 3. Indeksy

```text
IX_<TableName>_<ColumnName>
```

Przykłady:

```text
IX_Patients_DepartmentId
IX_MedicalRecords_PatientId
IX_UserDepartmentAccess_ApplicationUserId
```

## 4. Ograniczenia unikalne

```text
UQ_<TableName>_<ColumnName>
```

Przykłady:

```text
UQ_Departments_Code
UQ_ApplicationUsers_DomainLogin
UQ_Patients_MedicalNumber
```

## 5. Klucze obce

```text
FK_<ChildTable>_<ParentTable>_<ColumnName>
```

Przykłady:

```text
FK_Patients_Departments_DepartmentId
FK_MedicalRecords_Patients_PatientId
```

## 6. Role bazodanowe

```text
db_hac_<obszar>
```

Przykłady:

```text
db_hac_doctor
db_hac_nurse
db_hac_auditor
db_hac_app_runtime
```
