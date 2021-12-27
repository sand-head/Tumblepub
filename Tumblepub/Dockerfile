#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Tumblepub/Tumblepub.csproj", "Tumblepub/"]
RUN dotnet restore "Tumblepub/Tumblepub.csproj"
COPY . .
WORKDIR "/src/Tumblepub"
RUN dotnet build "Tumblepub.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tumblepub.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tumblepub.dll"]