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
COPY ["WebUI/WebSellerUI/WebSellerUI.csproj", "WebUI/WebSellerUI/"]
RUN dotnet restore "WebUI/WebSellerUI/WebSellerUI.csproj"
COPY . .
WORKDIR /src/WebUI/WebSellerUI
RUN dotnet build "WebSellerUI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebSellerUI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebSellerUI.dll"]