FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as build
WORKDIR /nsse-backend
COPY . .
RUN dotnet publish /nsse-backend/src/workers/NotSoSimpleEcommerce.InvoiceGenerator/NotSoSimpleEcommerce.InvoiceGenerator.csproj \
    -c Release \
    -o publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine
WORKDIR /nsse-backend
COPY --from=build /nsse-backend/publish .
EXPOSE 443

RUN adduser dotnet --disabled-password
RUN apk add curl 

USER dotnet
HEALTHCHECK CMD curl -k --fail https://localhost:443/invoice/health
ENTRYPOINT ["dotnet", "/nsse-backend/NotSoSimpleEcommerce.InvoiceGenerator.dll"]

