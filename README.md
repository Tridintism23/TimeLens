# \## Setup

# 

# 1\. Copy `appsettings.Development.example.json` thành `appsettings.Development.json`

# 2\. Điền Groq API key tại \[console.groq.com](https://console.groq.com)

# 3\. Chạy migration: `dotnet ef database update --project src/TimeLens.Infrastructure --startup-project src/TimeLens.WebAPI`

# 4\. Chạy app: `dotnet run --project src/TimeLens.WebAPI`

