# Sistema de Gestión de Envíos - API

Backend del sistema integrado para el control de logística, administración de empresas, envíos y pilotos. El proyecto está construido bajo los principios de la **Arquitectura Onion**.


## Tecnologías y Versiones Utilizadas

* **Framework Base:** .NET 8.0 (Target Framework: `net8.0`)
* **SDK Mínimo Requerido:** .NET SDK 8.0
* **ORM:** Entity Framework Core 8.0.6 (SQL Server)
* **Documentación de API:** Swashbuckle ASP.NET Core 6.6.0 (OpenAPI 3.0)



## Requisitos Previos

Antes de comenzar, asegúrate de tener instalado en tu máquina local:

1. **SDK de .NET 8.0** (Es indispensable contar con la versión 8 para evitar conflictos de compilación).
2. **SQL Server** o **SQL Server Express**.
3. **Visual Studio 2026** (con la carga de trabajo de *Desarrollo de ASP.NET y web* instalada).



## Pasos para la Configuración Inicial

Sigue estos pasos en orden estricto para montar el entorno en tu computadora local.

### 1. Clonar el repositorio

Abre tu terminal y clona el proyecto en tu carpeta de preferencia:

```bash
git clone <URL_DE_TU_REPOSITORIO_EN_GITHUB>
cd shipping-rest-api
```

### 2. Restaurar los Paquetes NuGet

Para descargar todas las dependencias del proyecto (Entity Framework, controladores, herramientas de desarrollo), ejecuta el siguiente comando en la raíz de la solución:

```bash
dotnet restore
```
*Nota: También puedes usar `dotnet build` para restaurar y verificar que todo compile correctamente de un solo golpe.*

### 3. Configurar la Cadena de Conexión Local

El archivo `appsettings.Development.json` contiene la configuración específica de tu base de datos.

Para configurar tu conexión:

1. Navega al proyecto **API**.

2. Localiza el archivo plantilla llamado **`appsettings.Development.Template.json`**.
3. Duplica ese archivo y renombra la copia exacta como: **`appsettings.Development.json`**.
4. Abre tu nuevo archivo `appsettings.Development.json` y modifica la propiedad `Server` colocando el nombre de tu instancia local de SQL Server.

Ejemplo de estructura correcta en tu archivo local:

```json
{
  "ConnectionStrings": {
    "CadenaConexion": "Server=TU_INSTANCIA_SQL\\SQLEXPRESS;Database=SistemaEnviosDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```
---

## Ejecución del Proyecto

### Desde Visual Studio

1. Abre el archivo de la solución `SistemaEnvios.sln`.
2. Asegúrate de que el proyecto **API** esté seleccionado como el "Proyecto de inicio".
3. En la barra superior, selecciona el perfil **https**.
4. Presiona **F5**. Visual Studio levantará el servidor web y abrirá automáticamente tu navegador en la interfaz gráfica de Swagger UI (`https://localhost:7075/`).


## Estructura de la Arquitectura Onion

El proyecto se divide de forma estricta en las siguientes capas independientes:

* **Domain (El Núcleo):** Contiene las entidades puras del negocio (`Entities/`) y los contratos lógicos (`Interfaces/`). No tiene dependencias externas de frameworks.
* **Application (Reglas de Negocio):** Orquesta los flujos de datos del sistema a través de Servicios (`Services/`) y expone estructuras limpias mediante Objetos de Transferencia de Datos (`DTOS/`).
* **Infrastructure (Acceso a Datos):** Implementa las interfaces de persistencia, gestiona los repositorios reales (`Repositories/`) y aloja el contexto de Entity Framework (`Data/AppDbContext.cs`).
* **API (Presentación):** Expone las rutas HTTP mediante controladores (`Controllers/`), lee las configuraciones locales y renderiza la documentación interactiva en Swagger.