# MyLibrary

```Bash
$ dotnet clean && dotnet build
$ dotnet pack --configuration Release
$ dotnet nuget push "Hogar.Core.Shared/bin/Release/Hogar.Core.Shared50.1.0.0.nupkg" -s "github"
$ dotnet nuget add source https://nuget.pkg.github.com/USUARIO/index.json --name github --username USUARIO --password TU_TOKEN --store-password-in-clear-text
```

Librería de ejemplo .NET 5 lista para publicar en GitLab Package Registry.

```Bash
$ git status
$ git add "xxx"
$ git commit -m "Comment"
$ dotnet sonarscanner begin -k:"Bankaool_Artifacts" -d:sonar.host.url="http://localhost:9000" -d:sonar.token="TOKEN" -d:sonar.coverageReportPaths="TestResults/sonarqubecoverage/SonarQube.xml"
$ dotnet clean && dotnet restore --force
$ dotnet build Bankaool.Core.Shared/Bankaool.Core.Shared.csproj & dotnet build Bankaool.Core.Shared.Tests/Bankaool.Core.Shared.Tests.csproj
$ dotnet test Bankaool.Core.Shared.Tests/Bankaool.Core.Shared.Tests.csproj --collect:"XPlat Code Coverage" --results-directory "TestResults"
$ reportgenerator "-reports:TestResults/**/coverage.cobertura.xml" "-targetdir:TestResults/reports" -reporttypes:Html
$ reportgenerator "-reports:TestResults/**/coverage.cobertura.xml" "-targetdir:TestResults/sonarqubecoverage" -reporttypes:SonarQube
$ dotnet sonarscanner end -d:sonar.token="TOKEN"
```

Registrar líbreria globalmente:

```Bash
$ dotnet nuget add source https://nuget.pkg.github.com/USUARIO/index.json --name github --username USUARIO --password TU_TOKEN --store-password-in-clear-text
```
