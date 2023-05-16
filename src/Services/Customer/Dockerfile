#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Services/Customer/Customer.csproj", "Services/Customer/"]
RUN dotnet restore "Services/Customer/Customer.csproj"
COPY . .
WORKDIR /src/Services/Customer/
RUN dotnet build "Customer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Customer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Customer.dll"]