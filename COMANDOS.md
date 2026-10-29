# Comandos de migración utilizados (VS Code / dotnet CLI)

```
dotnet tool install --global dotnet-ef

dotnet ef migrations add InitialCreate
dotnet ef database update

dotnet ef migrations add AgregarModalidadEvento
# (en el archivo generado: defaultValue: "" -> defaultValue: "Presencial")
dotnet ef database update

dotnet ef migrations add AgregarTelefonoAsistente
dotnet ef database update
```

# Consulta de evidencia (SQL Server, base EventosDB)

```sql
SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory ORDER BY MigrationId;
```

Conexión usada: `Server=(localdb)\MSSQLLocalDB;Database=EventosDB;Trusted_Connection=True;TrustServerCertificate=True`
