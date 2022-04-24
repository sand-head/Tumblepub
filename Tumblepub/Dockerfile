# See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Prevent 'Warning: apt-key output should not be parsed (stdout is not a terminal)'
ENV APT_KEY_DONT_WARN_ON_DANGEROUS_USAGE=1

# Install NodeJS 16.x (active LTS)
RUN apt-get update -yq 
RUN apt-get install curl gnupg -yq 
RUN curl -sL https://deb.nodesource.com/setup_16.x | bash -
RUN apt-get install -y nodejs

COPY ["Tumblepub.ActivityPub/Tumblepub.ActivityPub.csproj", "Tumblepub.ActivityPub/"]
COPY ["Tumblepub.Application/Tumblepub.Application.csproj", "Tumblepub.Application/"]
COPY ["Tumblepub.Infrastructure/Tumblepub.Infrastructure.csproj", "Tumblepub.Infrastructure/"]
COPY ["Tumblepub.Themes/Tumblepub.Themes.csproj", "Tumblepub.Themes/"]
COPY ["theme", "theme/"]
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
