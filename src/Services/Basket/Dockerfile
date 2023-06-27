#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS base
WORKDIR /app
EXPOSE 80

COPY [ "Basket.csproj", "./Services/Basket/"]
RUN dotnet restore ./Services/Basket/Basket.csproj
COPY . ./Services/Basket
WORKDIR /app/Services/Basket/
RUN ls
RUN dotnet build "Basket.csproj" -c Release -o /app/build
RUN dotnet publish "Basket.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=base /app/publish .
ENTRYPOINT ["dotnet", "Basket.dll"]"

