# Artifact Base --> Common Library

Librería de ejemplo .NET 5 y .NET 8 lista para publicar en GitLab Package Registry.

Primera compilación y con code coverage:

```Bash
$ dotnet tool install --global dotnet-reportgenerator-globaltool
$ dotnet clean && dotnet restore --force && dotnet build
$ rm -rf coverage coverageHistory TestResults
$ dotnet test Utilities.Core.Shared.Tests/Utilities.Core.Shared.Tests.csproj --collect:"XPlat Code Coverage" --results-directory "TestResults"
$ reportgenerator "-reports:TestResults/**/coverage.cobertura.xml" "-targetdir:TestResults/reports" -reporttypes:Html
```

Compilación general:

```Bash
$ dotnet clean && dotnet build
$ dotnet pack --configuration Release
$ dotnet nuget push "Utilities.Core.Shared/bin/Release/Utilities.Core.Shared50.1.0.0.nupkg" -s "github" --api-key "YOUR_TOKEN"
$ dotnet nuget add source https://nuget.pkg.github.com/USUARIO/index.json --name github --username USUARIO --password TU_TOKEN --store-password-in-clear-text
```

Para Nuget:
```Bash
$ dotnet clean && dotnet restore --force && dotnet build
$ dotnet pack --configuration Release
$ dotnet nuget push bin/Release/MiBiblioteca.1.0.0.nupkg --api-key TU_API_KEY --source https://api.nuget.org/v3/index.json
```

SonarQube:
```Bash
$ git status
$ git add "xxx"
$ git commit -m "Comment"
$ dotnet sonarscanner begin -k:"Bankaool_Artifacts" -d:sonar.host.url="http://localhost:9000" -d:sonar.token="TOKEN" -d:sonar.coverageReportPaths="TestResults/sonarqubecoverage/SonarQube.xml"
$ dotnet clean && dotnet restore --force
$ dotnet build Utilities.Core.Shared/Utilities.Core.Shared.csproj & dotnet build Utilities.Core.Shared.Tests/Utilities.Core.Shared.Tests.csproj
$ dotnet test Utilities.Core.Shared.Tests/Utilities.Core.Shared.Tests.csproj --collect:"XPlat Code Coverage" --results-directory "TestResults"
$ reportgenerator "-reports:TestResults/**/coverage.cobertura.xml" "-targetdir:TestResults/reports" -reporttypes:Html
$ reportgenerator "-reports:TestResults/**/coverage.cobertura.xml" "-targetdir:TestResults/sonarqubecoverage" -reporttypes:SonarQube
$ dotnet sonarscanner end -d:sonar.token="TOKEN"
```

Registrar líbreria globalmente:

```Bash
$ dotnet nuget add source https://nuget.pkg.github.com/USUARIO/index.json --name github --username USUARIO --password TU_TOKEN --store-password-in-clear-text
```
