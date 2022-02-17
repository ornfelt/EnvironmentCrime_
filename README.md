# EnvironmentCrime - Final version for school

- Dotnet project using entity framework 2.2

# Notes

- You need to create the databases "EnvironmentCrime" and "Identity" in visual studio and do:
- dotnet ef migrations add Initial --context ApplicationDbContext
- dotnet ef database update --context ApplicationDbContext
- dotnet ef migrations add Initial --context AppIdentity --context AppIdentityDbContext
- dotnet ef database update --context AppIdentityDbContext
