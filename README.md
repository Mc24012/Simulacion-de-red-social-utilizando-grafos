# Simulación de Red Social utilizando Grafos

Aplicación WPF (.NET 8) que simula una red social usando teoría de grafos para modelar relaciones entre usuarios y generar sugerencias de amistades.

## Características

- **Modelado de grafos**: Usuarios como nodos, relaciones como aristas
- **Sugerencias de amistad**: Algoritmos basados en conexiones mutuas y intereses comunes
- **Visualización gráfica**: Interfaz interactiva con GraphX
- **Persistencia**: Base de datos MySQL para usuarios, relaciones e intereses
- **CRUD completo**: Gestión de usuarios, relaciones e intereses

## Tecnologías

- .NET 8 / WPF
- GraphX (visualización de grafos)
- MySQL (MySqlConnector)
- Arquitectura en capas: Modelos, DAOs, Servicios, UI

## Estructura del proyecto

```
├── Modelos/           # Entidades de dominio (Usuario, Relacion, GrafoSocial)
├── DAOs/              # Acceso a datos (UsuarioDAO, RelacionDAO, InteresDAO)
├── Servicios/         # Lógica de negocio (ServicioGrafo, ServicioSugerencias)
├── Clases/            # Estructuras de grafo (Grafo, Relacion, Usuario)
├── UI/                # Componentes visuales (VisualizadorGrafo)
├── Assets/            # Imágenes
├── BaseDatos/         # Conexión MySQL
├── VentanaGrafo.xaml  # Ventana principal de visualización
├── MainWindow.xaml    # Ventana de gestión de usuarios
└── red_db.sql         # Esquema de base de datos
```

## Base de datos (XAMPP / MySQL)

La cadena de conexión en `BaseDatos/Conexion.cs` ya está configurada para XAMPP por defecto:
- Server: 127.0.0.1 (localhost)
- Port: 3306
- User: root
- Password: (vacío)
- Database: red_db

**Pasos en XAMPP:**

1. Iniciar **Apache** y **MySQL** en XAMPP Control Panel
2. Abrir **phpMyAdmin**: http://localhost/phpmyadmin
3. Crear base de datos `red_db` (collation: utf8mb4_general_ci)
4. Seleccionar `red_db` → pestaña **Importar** → elegir `red_db.sql` → Continuar

> **Nota:** El archivo SQL no crea la base de datos, solo las tablas/procedimientos. Debes crearla manualmente en phpMyAdmin primero.

## Ejecutar

1. Clonar repositorio
2. Iniciar MySQL en XAMPP Control Panel
3. Crear BD `red_db` e importar `red_db.sql` en phpMyAdmin
4. Abrir `.sln` en Visual Studio 2022+
5. Compilar y ejecutar (F5)

## Requisitos

- .NET 8 SDK
- XAMPP (MySQL/MariaDB) o MySQL Server
- Visual Studio 2022+ con workload WPF