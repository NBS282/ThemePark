# Utilizar la imagen base de .NET Core SDK para compilar la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Establecer el directorio de trabajo dentro del contenedor
WORKDIR /source

# Copiar el archivo de solución y los archivos de proyecto
COPY ThemePark.sln .
COPY ThemeParkApi/ThemeParkApi.csproj ThemeParkApi/
COPY ThemePark.BusinessLogic/ThemePark.BusinessLogic.csproj ThemePark.BusinessLogic/
COPY ThemePark.IBusinessLogic/ThemePark.IBusinessLogic.csproj ThemePark.IBusinessLogic/
COPY ThemePark.DataAccess/ThemePark.DataAccess.csproj ThemePark.DataAccess/
COPY ThemePark.IDataAccess/ThemePark.IDataAccess.csproj ThemePark.IDataAccess/
COPY ThemePark.Domain/ThemePark.Domain.csproj ThemePark.Domain/
COPY ThemePark.Models/ThemePark.Models.csproj ThemePark.Models/
COPY ThemePark.ServiceFactory/ThemePark.ServiceFactory.csproj ThemePark.ServiceFactory/

# Restaurar las dependencias
RUN dotnet restore "ThemeParkApi/ThemeParkApi.csproj" --disable-parallel

# Copiar el resto de los archivos del proyecto
COPY . .

# Compilar y publicar la aplicación
RUN dotnet publish "ThemeParkApi/ThemeParkApi.csproj" -c Release -o /app --no-restore

# Etapa de producción
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copiar los archivos publicados de la etapa anterior
COPY --from=build /app ./

# Copiar el directorio de plugins si existe
COPY --from=build /source/ThemeParkApi/Plugins ./Plugins

# Exponer el puerto 8080
EXPOSE 8080

# Comando para ejecutar la aplicación
ENTRYPOINT ["dotnet", "ThemeParkApi.dll"]
