## Acerca de esta solución

Esta es una solución por capas basada en prácticas de [Domain Driven Design (DDD)](https://abp.io/docs/latest/framework/architecture/domain-driven-design). Todos los módulos fundamentales de ABP ya están instalados. Consulte la documentación de [Application Startup Template](https://abp.io/docs/latest/solution-templates/layered-web-application) para más información.

### Pre-requisitos

* [.NET 9.0+ SDK](https://dotnet.microsoft.com/download/dotnet)
* [Node v18 o 20](https://nodejs.org/en)
* [SQL Server](https://www.microsoft.com/sql-server) (LocalDB o instancia completa)

### Configuraciones

La solución viene con una configuración predeterminada que funciona de inmediato. Sin embargo, puede considerar cambiar la siguiente configuración antes de ejecutar su solución:

* Verifique los `ConnectionStrings` en los archivos `appsettings.json` bajo los proyectos `TurisGo.HttpApi.Host` y `TurisGo.DbMigrator` y cámbielos si es necesario.

### Antes de ejecutar la aplicación

* Ejecute el comando `abp install-libs` en la carpeta de su solución para instalar las dependencias de paquetes del lado del cliente. Este paso se realiza automáticamente cuando crea una nueva solución, si no lo deshabilitó especialmente. Sin embargo, debe ejecutarlo usted mismo si ha clonado esta solución por primera vez desde su control de código fuente, o si ha agregado una nueva dependencia de paquete del lado del cliente a su solución.

* Ejecute `TurisGo.DbMigrator` para crear la base de datos inicial. Este paso también se realiza automáticamente cuando crea una nueva solución, si no lo deshabilitó especialmente. Esto debe hacerse en la primera ejecución. También es necesario si se agrega una nueva migración de base de datos a la solución más adelante.

#### Generación de un Certificado de Firma

En el entorno de producción, necesita usar un certificado de firma de producción. ABP Framework configura certificados de firma y cifrado en su aplicación y espera un archivo `openiddict.pfx` en su aplicación.

Para generar un certificado de firma, puede usar el siguiente comando:

```bash
dotnet dev-certs https -v -ep openiddict.pfx -p 8957eae5-cbae-499f-8aaa-1431dfc8e183
```

> `8957eae5-cbae-499f-8aaa-1431dfc8e183` es la contraseña del certificado, puede cambiarla a cualquier contraseña que desee.

Se recomienda usar **dos** certificados RSA, distintos del certificado(s) usado(s) para HTTPS: uno para cifrado, uno para firma.

Para más información, consulte: [OpenIddict Certificate Configuration](https://documentation.openiddict.com/configuration/encryption-and-signing-credentials.html#registering-a-certificate-recommended-for-production-ready-scenarios)

> También, consulte la documentación de [Configuring OpenIddict](https://abp.io/docs/latest/Deployment/Configuring-OpenIddict#production-environment) para más información.

### Estructura de la solución

Esta es una aplicación monolítica por capas que consiste en las siguientes aplicaciones:

* `TurisGo.DbMigrator`: Una aplicación de consola que aplica las migraciones y también siembra los datos iniciales. Es útil tanto en desarrollo como en entorno de producción.
* `TurisGo.HttpApi.Host`: Aplicación ASP.NET Core API que se usa para exponer las APIs a los clientes.
* `angular`: Aplicación Angular.

### Estructura de capas (DDD)

```
src/
├── TurisGo.Domain.Shared/          # Kernel compartido (Enums, Constantes)
├── TurisGo.Domain/                 # Capa de Dominio (Entidades, Lógica de negocio)
├── TurisGo.Application.Contracts/  # Contratos (Interfaces, DTOs)
├── TurisGo.Application/            # Capa de Aplicación (Servicios)
├── TurisGo.EntityFrameworkCore/    # Infraestructura (EF Core, Repositorios)
├── TurisGo.HttpApi/                # API HTTP (Controladores)
└── TurisGo.HttpApi.Host/           # Host de la aplicación
```

## Despliegue de la aplicación

Desplegar una aplicación ABP sigue el mismo proceso que desplegar cualquier aplicación .NET o ASP.NET Core. Sin embargo, hay consideraciones importantes a tener en cuenta. Para una guía detallada, consulte la [documentación de despliegue](https://abp.io/docs/latest/Deployment/Index) de ABP.

### Recursos adicionales

#### Recursos Internos

Puede encontrar guías detalladas de configuración para su solución a continuación:

* [Angular](./angular/README.md)
* [Manual de Usuario](./docs/Manual_de_Usuario_TurisGo.md)

#### Recursos Externos

Puede ver los siguientes recursos para aprender más sobre su solución y ABP Framework:

* [Web Application Development Tutorial](https://abp.io/docs/latest/tutorials/book-store/part-1)
* [Application Startup Template](https://abp.io/docs/latest/startup-templates/application/index)
* [Domain Driven Design](https://abp.io/docs/latest/framework/architecture/domain-driven-design)
