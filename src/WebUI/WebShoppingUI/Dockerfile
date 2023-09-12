FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx
RUN curl -sL https://deb.nodesource.com/setup_lts.x | bash -
RUN apt-get install -y nodejs

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx
RUN curl -sL https://deb.nodesource.com/setup_lts.x | bash -
RUN apt-get install -y nodejs
WORKDIR /src
COPY ["WebUI/WebShoppingUI/WebShoppingUI.csproj", "WebUI/WebShoppingUI/"]
RUN dotnet restore "WebUI/WebShoppingUI/WebShoppingUI.csproj"
COPY . .
WORKDIR /src/WebUI/WebShoppingUI
RUN dotnet build "WebShoppingUI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebShoppingUI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebShoppingUI.dll"]