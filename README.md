# EnvironmentCrime - Final version for school

- Dotnet project using entity framework 2.2 (now 7.0)

# Notes

Not sure about these steps... Try to just do this:
- dotnet ef migrations add Initial --context ApplicationDbContext
- dotnet ef database update --context ApplicationDbContext
- dotnet ef migrations add Initial --context AppIdentityDbContext
- dotnet ef database update --context AppIdentityDbContext

You need these nuget packages:
- Microsoft.EntityFrameworkCore.Tools
- Microsoft.EntityFrameworkCore.Tools.Dotnet
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Design
- Microsoft.EntityFrameworkCore
- Microsoft.AspNetCore.Razor.Design
- Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation
- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- Microsoft.AspNetCore.Identity
- Newtonsoft.Json

Old:
- You need to create the databases "EnvironmentCrime" and "Identity" in visual studio and do:
- dotnet ef migrations add Initial --context ApplicationDbContext
- dotnet ef database update --context ApplicationDbContext
- dotnet ef migrations add Initial --context AppIdentity --context AppIdentityDbContext
- dotnet ef database update --context AppIdentityDbContext
