FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ExcalibPayments.Orders.Web/ExcalibPayments.Orders.Web.csproj", "ExcalibPayments.Orders.Web/"]
COPY ["ExcalibPayments.Orders.Application/ExcalibPayments.Orders.Application.csproj", "ExcalibPayments.Orders.Application/"]
COPY ["ExcalibPayments.Orders.Domain/ExcalibPayments.Orders.Domain.csproj", "ExcalibPayments.Orders.Domain/"]
RUN dotnet restore "ExcalibPayments.Orders.Web/ExcalibPayments.Orders.Web.csproj"
COPY . .
WORKDIR "/src/ExcalibPayments.Orders.Web"
RUN dotnet build "ExcalibPayments.Orders.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ExcalibPayments.Orders.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExcalibPayments.Orders.Web.dll"]
