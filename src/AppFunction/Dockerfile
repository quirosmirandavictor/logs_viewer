FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["AppFunction.csproj", "./"]
RUN dotnet restore "./AppFunction.csproj"

COPY . .
RUN dotnet publish "./AppFunction.csproj" -c Release -o /home/site/wwwroot /p:UseAppHost=false

FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0
WORKDIR /home/site/wwwroot
COPY --from=build /home/site/wwwroot .
