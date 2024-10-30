

dotnet tool install --global dotnet-ef --version 8.0.10

dotnet ef migrations add InitialCreate

dotnet ef database update