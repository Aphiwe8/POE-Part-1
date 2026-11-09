# Dockerfiles are used to create a container image. 
#the images can be used to run the project in containerised enviro.
#A containerised envrionment is a light setup that keep s all dependencies in a container.
#It uses less storage and isilates applications so that security issues do not spread


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS installer-env
WORKDIR /src
COPY . .
RUN dotnet publish CoffeeAndChill.csproj -c Release -o /home/site/wwwroot

FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true
COPY --from=installer-env ["/home/site/wwwroot", "/home/site/wwwroot"]