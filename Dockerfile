FROM alpine/git as version
WORKDIR /src
COPY . /src
RUN echo $(git describe --tags --always 2>/dev/null | sed 's/-g[a-z0-9]\{7\}//') > /version ;\
    echo "Version: "$(cat /version)

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
COPY . /build
COPY --from=version /version /build/version
WORKDIR /build
RUN apt-get update -yq &&\
	apt-get install curl gnupg -yq &&\
	curl -fsSL https://deb.nodesource.com/setup_20.x | bash - &&\
	apt-get install -y nodejs
ENV NODE_OPTIONS=--openssl-legacy-provider
RUN sed -i -e "s/<Version>0-develop<\/Version>/<Version>$(cat version | cut -c2- )<\/Version>/g" src/Influunt.Host/Influunt.Host.csproj &&\
    dotnet restore -s https://api.nuget.org/v3/index.json &&\
    dotnet build --no-restore -c Release &&\    
    dotnet publish ./src/Influunt.Host/Influunt.Host.csproj -c Release -o /app --no-build &&\
    dotnet nuget locals http-cache --clear &&\
    dotnet nuget locals temp --clear


######## Influunt Host
FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim as influunt
COPY --from=build /app /influunt
WORKDIR /influunt
EXPOSE 80
ENV ASPNETCORE_URLS=http://*:80
ENTRYPOINT ["dotnet", "Influunt.Host.dll"]
