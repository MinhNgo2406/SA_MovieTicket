FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Chỉ copy file csproj và restore trước để tận dụng cache
COPY ["MovieTicket.csproj", "./"]
RUN dotnet restore "MovieTicket.csproj"

# 2. Copy toàn bộ các thư mục quan trọng (bao gồm cả Protos)
COPY . .

# 3. Build ứng dụng
RUN dotnet build "MovieTicket.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MovieTicket.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MovieTicket.dll"]