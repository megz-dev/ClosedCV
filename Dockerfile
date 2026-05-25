FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["src/ClosedCV.Api/ClosedCV.Api.csproj", "src/ClosedCV.Api/"]
COPY ["src/ClosedCV.Application/ClosedCV.Application.csproj", "src/ClosedCV.Application/"]
COPY ["src/ClosedCV.Domain/ClosedCV.Domain.csproj", "src/ClosedCV.Domain/"]
COPY ["src/ClosedCV.Infrastructure/ClosedCV.Infrastructure.csproj", "src/ClosedCV.Infrastructure/"]
RUN dotnet restore "src/ClosedCV.Api/ClosedCV.Api.csproj"
COPY . .
WORKDIR "/src/src/ClosedCV.Api"
RUN dotnet build "ClosedCV.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ClosedCV.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ClosedCV.Api.dll"]