# See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG TARGETARCH
WORKDIR /src

# Prevent 'Warning: apt-key output should not be parsed (stdout is not a terminal)'
ENV APT_KEY_DONT_WARN_ON_DANGEROUS_USAGE=1

# Install NodeJS 16.x (active LTS)
RUN apt-get update -yq 
RUN apt-get install curl gnupg -yq 
RUN curl -sL https://deb.nodesource.com/setup_16.x | bash -
RUN apt-get install -y nodejs

COPY ["Tumblepub.Application/Tumblepub.Application.csproj", "Tumblepub.Application/"]
COPY ["Tumblepub.Infrastructure/Tumblepub.Infrastructure.csproj", "Tumblepub.Infrastructure/"]
COPY ["Tumblepub.ActivityPub/Tumblepub.ActivityPub.csproj", "Tumblepub.ActivityPub/"]
COPY ["Tumblepub.Themes/Tumblepub.Themes.csproj", "Tumblepub.Themes/"]
COPY ["theme", "theme/"]
COPY ["Tumblepub.Components/Tumblepub.Components.csproj", "Tumblepub.Components/"]
COPY ["Tumblepub/Tumblepub.csproj", "Tumblepub/"]
RUN dotnet restore "Tumblepub/Tumblepub.csproj" -a $TARGETARCH
COPY . .
WORKDIR "/src/Tumblepub"
RUN dotnet publish "Tumblepub.csproj" -a $TARGETARCH -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=build /app .
EXPOSE 80
ENTRYPOINT ["dotnet", "Tumblepub.dll"]
