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

## Base de datos

Ejecutar `red_db.sql` en MySQL para crear las tablas necesarias:

```sql
-- Tablas: usuarios, relaciones, intereses, usuario_intereses
```

Configurar conexión en `BaseDatos/Conexion.cs`.

## Ejecutar

1. Clonar repositorio
2. Configurar connection string en `BaseDatos/Conexion.cs`
3. Ejecutar `red_db.sql` en MySQL
4. Abrir `.sln` en Visual Studio 2022+
5. Compilar y ejecutar (F5)

## Requisitos

- .NET 8 SDK
- MySQL Server
- Visual Studio 2022+ con workload WPF