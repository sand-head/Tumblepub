FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Tumblepub.FSharp/Tumblepub.FSharp.fsproj", "Tumblepub.FSharp/"]
RUN dotnet restore "Tumblepub.FSharp/Tumblepub.FSharp.fsproj"
COPY . .
WORKDIR "/src/Tumblepub.FSharp"
RUN dotnet build "Tumblepub.FSharp.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tumblepub.FSharp.fsproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tumblepub.FSharp.dll"]
