FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["src/ClosedCV.Api/ClosedCV.Api.csproj", "ClosedCV.Api/"]
COPY ["src/ClosedCV.Application/ClosedCV.Application.csproj", "ClosedCV.Application/"]
COPY ["src/ClosedCV.Domain/ClosedCV.Domain.csproj", "ClosedCV.Domain/"]
COPY ["src/ClosedCV.Infrastructure/ClosedCV.Infrastructure.csproj", "ClosedCV.Infrastructure/"]
RUN dotnet restore "ClosedCV.Api/ClosedCV.Api.csproj"
COPY src/ .
WORKDIR "/src/ClosedCV.Api"
RUN dotnet build "ClosedCV.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ClosedCV.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ClosedCV.Api.dll"]