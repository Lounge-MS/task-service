FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY *.slnx ./
COPY Directory.*.props ./
COPY src/TaskServiceDomain/TaskServiceDomain.csproj src/TaskServiceDomain/
COPY src/TaskServiceInfrastructure/TaskServiceInfrastructure.csproj src/TaskServiceInfrastructure/
COPY src/TaskServiceKafka/TaskServiceKafka.csproj src/TaskServiceKafka/

RUN dotnet restore

COPY . .

RUN dotnet publish src/TaskServiceKafka/TaskServiceKafka.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

COPY src/TaskServiceKafka/appsettings*.json ./

ENTRYPOINT ["dotnet", "TaskServiceKafka.dll"]

