FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Naturino.sln ./
COPY src/Naturino.Domain/Naturino.Domain.csproj src/Naturino.Domain/
COPY src/Naturino.Application/Naturino.Application.csproj src/Naturino.Application/
COPY src/Naturino.Infrastructure/Naturino.Infrastructure.csproj src/Naturino.Infrastructure/
COPY src/Naturino.API/Naturino.API.csproj src/Naturino.API/
RUN dotnet restore src/Naturino.API/Naturino.API.csproj

COPY src/ src/
RUN dotnet publish src/Naturino.API/Naturino.API.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Naturino.API.dll"]
