FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /app

# Copying csproj and restore for docker caching if files did not change
COPY ./ExpenseMailService.Api/*.csproj ./
COPY ./ExpenseMailService.Api.Tests/*.csproj ./
RUN dotnet restore ExpenseMailService.Api.csproj
RUN dotnet restore ExpenseMailService.Api.Tests.csproj

# Copying after restore to grab the latest changes
COPY . ./
RUN dotnet test ExpenseMailService.Api.Tests -c Release
RUN dotnet publish ExpenseMailService.Api -c Release -o /app/out

FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://*:8080
EXPOSE 8080
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "ExpenseMailService.Api.dll"]