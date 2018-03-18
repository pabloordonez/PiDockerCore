FROM microsoft/dotnet:2.0-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /src
COPY PiDockerCore.sln ./
COPY PiDockerCore/PiDockerCore.csproj PiDockerCore/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/PiDockerCore
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c release -o /app -r linux-arm

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "PiDockerCore.dll"]
