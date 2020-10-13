FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS build

WORKDIR /app
COPY *.sln .
COPY src/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p src/${file%.*}/ && mv $file src/${file%.*}/; done
COPY tests/*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p tests/${file%.*}/ && mv $file tests/${file%.*}/; done
RUN dotnet restore --packages nuget/packages
COPY . .
RUN dotnet build -c Release --no-restore  --packages nuget/packages

FROM build AS test
RUN dotnet test --logger:trx

FROM build AS publish
RUN dotnet publish src/kAttendance/kAttendance.csproj -c Release --no-restore --no-build -o /output

FROM mcr.microsoft.com/dotnet/core/runtime:2.1 AS final
WORKDIR /app
COPY --from=publish /output .
ENTRYPOINT ["dotnet", "kAttendance.dll"]
