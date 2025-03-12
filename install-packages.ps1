cd DetectiveMysteryGamePlatform.Api

# Install required packages
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools

# Return to root directory
cd ..

# Install EF Core global tool if not already installed
dotnet tool install --global dotnet-ef

# Create initial migration
dotnet ef migrations add InitialCreate --project DetectiveMysteryGamePlatform.Api 