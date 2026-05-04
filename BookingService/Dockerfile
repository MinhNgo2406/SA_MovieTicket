# Sử dụng môi trường runtime để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Sử dụng SDK để build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# COPY file .csproj vào và restore (LƯU Ý: Thay đổi tên file theo từng folder)
COPY ["BookingService.csproj", "./"]
RUN dotnet restore "BookingService.csproj"

# Copy toàn bộ code còn lại và build
COPY . .
RUN dotnet build "BookingService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BookingService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage cuối cùng để chạy app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# LƯU Ý: Thay đổi tên .dll theo project của bạn
ENTRYPOINT ["dotnet", "BookingService.dll"]