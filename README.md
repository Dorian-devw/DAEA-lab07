# 📚 Sistema de Gestión de Biblioteca (WPF + ADO.NET)

Un sistema de gestión de bibliotecas desarrollado en **C# con .NET 10**, estructurado en una **Arquitectura de 4 Capas** (Entidades, Datos, Negocio y Presentación WPF). Este proyecto implementa lógica de negocio compleja, transacciones ACID, y utiliza diseño UI moderno gracias a la librería **HandyControl**.

## 🚀 Arquitectura del Proyecto (4 Capas)

El proyecto está rigurosamente dividido respetando la Inversión de Dependencias (DI) e Interfaces:

1. **`Biblioteca.Entidades` (Entities):** POCOs puros que representan las tablas (Socios, Libros, Préstamos, etc.) y DTOs para transferencia de datos. No tiene referencias a otras capas.
2. **`Biblioteca.Datos` (Data):** Encargada de la persistencia de datos (CRUD) y consultas directas a SQL Server mediante ADO.NET (`SqlCommand`, `SqlDataReader`). Utiliza un patrón de Repositorio, implementando las Interfaces definidas en la capa de Negocio. No retorna objetos de ADO.NET (ni `DataTable` ni `SqlDataReader`) hacia arriba.
3. **`Biblioteca.Negocio` (Business/BLL):** Contiene todas las validaciones y reglas del sistema (límites, fechas, cálculos de multas). Utiliza excepciones personalizadas (`ReglaNegocioException`) para rechazar acciones inválidas.
4. **`Biblioteca.WPF` (Presentation):** La interfaz gráfica de usuario. Solo interactúa con la capa de `Negocio` y de `Entidades`. Aquí configuramos la Inyección de Dependencias a través de `Microsoft.Extensions.DependencyInjection`.

---

## 🛠️ Tecnologías y Requisitos

*   **IDE:** Microsoft Visual Studio 2026 (o compatible).
*   **Framework:** .NET 10.0 (Windows).
*   **Base de Datos:** Microsoft SQL Server (LocalDB, Express o superior).
*   **Diseño UI:** [HandyControl](https://handyorg.github.io/handycontrol/) (NuGet).
*   **Gestión de Dependencias:** `Microsoft.Extensions.DependencyInjection`.

---

## ⚙️ Instalación y Configuración

Sigue estos pasos para desplegar el proyecto localmente en tu computadora:

### 1. Clonar el repositorio
```bash
git clone https://github.com/TU_USUARIO/Biblioteca-WPF-4Capas.git
```

### 2. Configurar la Base de Datos (SQL Server)
1. Abre **SQL Server Management Studio (SSMS)**.
2. Abre el archivo de script incluido en la raíz del proyecto: `BibliotecaDB_Setup.sql`.
3. Ejecuta el script completo (tecla **F5**). 
   *(Este script creará la base de datos `BibliotecaDB`, las tablas relacionadas y poblará las tablas con datos semilla: autores, libros, socios y algunos préstamos pendientes para pruebas).*

### 3. Configurar la Cadena de Conexión
1. Abre la solución `Biblioteca.sln` en Visual Studio.
2. En el Explorador de Soluciones, dirígete al proyecto **`Biblioteca.WPF`**.
3. Abre el archivo **`App.config`**.
4. Modifica la propiedad `Data Source` con el nombre de tu instancia local de SQL Server. 
   *(Ejemplo: `Data Source=.\SQLEXPRESS;`, `Data Source=(localdb)\MSSQLLocalDB;` o el nombre de tu PC).*

```xml
<add name="BibliotecaDB" 
     connectionString="Data Source=TU_SERVIDOR_SQL;Initial Catalog=BibliotecaDB;Integrated Security=True;TrustServerCertificate=True" 
     providerName="System.Data.SqlClient"/>
```

### 4. Compilación e Inicio
1. Haz clic derecho sobre el proyecto **`Biblioteca.WPF`** y selecciona **"Establecer como proyecto de inicio"** (Set as Startup Project).
2. Ve al menú superior y selecciona **Compilar > Recompilar solución**. *(Esto descargará e instalará HandyControl y el resto de paquetes NuGet automáticamente).*
3. Presiona **F5** (o Iniciar) para ejecutar la aplicación.

---

## ✨ Características y Reglas de Negocio Implementadas

Este proyecto cubre exhaustivamente las reglas de negocio típicas de un sistema real:

*   🔒 **Límite de Préstamos:** Un socio solo puede tener un máximo de **3 libros** pendientes al mismo tiempo. Validado en la capa de Negocio (Regla 10).
*   📦 **Control de Stock y Transacciones:** Al prestar varios libros, se ejecuta una transacción `SqlTransaction`. Si el préstamo tiene éxito, se descuentan automáticamente los *Ejemplares* de la tabla `Libros`. Si ocurre un error, se aplica un `Rollback` completo.
*   💰 **Cálculo de Multas Automático:** Al devolver un libro, la capa de Negocio comprueba si la fecha actual sobrepasa la `FechaLimite`. De ser así, calcula una multa de **S/ 1.50 por cada día de retraso** (Regla 11) y se lo notifica a la interfaz.
*   🗑️ **Baja Lógica Protegida:** No se utilizan sentencias `DELETE`. Se usa un campo `Activo (BIT)` para "eliminar" registros (Regla 12). Además, un socio **no puede ser dado de baja** si tiene libros pendientes por devolver.
*   📊 **Reportes Avanzados:** Módulo de reportes implementado mediante consultas SQL limpias cruzando tablas (`INNER JOIN` entre Préstamos, Detalle, Libros y Socios) devolviendo un objeto `DTO` fuertemente tipado (`ReportePrestamoDto`) y filtrado por un rango de fechas.

---

## 🖼️ Capturas de Pantalla / Interfaz
*(Sugerencia: Una vez subas el código a GitHub, puedes pegar aquí capturas de pantalla de la interfaz WPF de tu aplicación, demostrando el uso de HandyControl y las ventanas funcionando).*

## 👨‍💻 Autor
Desarrollado para el Laboratorio Universitario.
*Diseño orientado a la separación de responsabilidades y buenas prácticas de ingeniería de software.*
