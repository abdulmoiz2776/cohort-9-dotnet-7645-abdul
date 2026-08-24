# SonarQube Backend Analysis

The `Backend SonarQube` workflow builds the .NET solution, runs the backend unit tests with OpenCover coverage, and waits for the SonarQube quality gate.

Configure these repository secrets before enabling the workflow:

- `SONAR_HOST_URL`: SonarQube server URL
- `SONAR_PROJECT_KEY`: project key configured in SonarQube
- `SONAR_TOKEN`: analysis token

Run the same checks locally from `backend/TaskManagement.API` after installing the scanner as a global .NET tool:

```powershell
dotnet sonarscanner begin /k:"project-key" /d:sonar.host.url="https://sonarqube.example.com" /d:sonar.token="$env:SONAR_TOKEN" /d:sonar.cs.opencover.reportsPaths="TestResults/**/coverage.opencover.xml"
dotnet build TaskManagement.API.slnx
dotnet test TaskManagement.Tests/TaskManagement.Tests.csproj --collect:"XPlat Code Coverage" --results-directory TestResults -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=opencover
dotnet sonarscanner end /d:sonar.token="$env:SONAR_TOKEN"
```