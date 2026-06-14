using System.Data;
using Simulacion_de_red_social_utilizando_grafos.Modelos;

namespace Simulacion_de_red_social_utilizando_grafos.DAOs
{
    public class UsuarioDAO : IGenerico<Usuario>
    {
        private readonly Func<IDbConnection> _crearConexion;

        public UsuarioDAO(Func<IDbConnection> crearConexion)
        {
            _crearConexion = crearConexion;
        }

        public List<Usuario> ObtenerTodos()
        {
            const string sql = "SELECT Id_Usuario, Nombre_Usuario, Genero_Usuario, Email_Usuario, Activo, Fecha_Eliminacion FROM usuarios";
            return ConsultarUsuarios(sql);
        }

        public List<Usuario> ObtenerActivos()
        {
            const string sql = """
                SELECT
                    Id_Usuario,
                    Nombre_Usuario,
                    Genero_Usuario,
                    Email_Usuario,
                    Activo,
                    Fecha_Eliminacion
                FROM usuarios
                WHERE Activo = 1
                  AND Fecha_Eliminacion IS NULL
                """;
            return ConsultarUsuarios(sql);
        }

        public Usuario? ObtenerPorId(int id)
        {
            const string sql = "SELECT Id_Usuario, Nombre_Usuario, Genero_Usuario, Email_Usuario, Activo, Fecha_Eliminacion FROM usuarios WHERE Id_Usuario = @id";

            using var conexion = _crearConexion();
            using var comando = CrearComando(conexion, sql);
            AgregarParametro(comando, "@id", id);

            conexion.Open();
            using var lector = comando.ExecuteReader();
            return lector.Read() ? MapearUsuario(lector) : null;
        }

        public void Insertar(Usuario entidad)
        {
            const string sql = """
                INSERT INTO usuarios (Nombre_Usuario, Genero_Usuario, Email_Usuario, Activo, Fecha_Eliminacion)
                VALUES (@nombre, @genero, @email, @activo, @fechaEliminacion)
                """;

            Ejecutar(sql, comando =>
            {
                AgregarParametro(comando, "@nombre", entidad.Nombre_Usuario);
                AgregarParametro(comando, "@genero", entidad.Genero_Usuario);
                AgregarParametro(comando, "@email", entidad.Email_Usuario);
                AgregarParametro(comando, "@activo", entidad.Activo);
                AgregarParametro(comando, "@fechaEliminacion", entidad.Fecha_Eliminacion);
            });
        }

        public void Actualizar(Usuario entidad)
        {
            const string sql = """
                UPDATE usuarios
                SET Nombre_Usuario = @nombre,
                    Genero_Usuario = @genero,
                    Email_Usuario = @email,
                    Activo = @activo,
                    Fecha_Eliminacion = @fechaEliminacion
                WHERE Id_Usuario = @id
                """;

            Ejecutar(sql, comando =>
            {
                AgregarParametro(comando, "@id", entidad.Id_Usuario);
                AgregarParametro(comando, "@nombre", entidad.Nombre_Usuario);
                AgregarParametro(comando, "@genero", entidad.Genero_Usuario);
                AgregarParametro(comando, "@email", entidad.Email_Usuario);
                AgregarParametro(comando, "@activo", entidad.Activo);
                AgregarParametro(comando, "@fechaEliminacion", entidad.Fecha_Eliminacion);
            });
        }

        public void Eliminar(int id)
        {
            Ejecutar("CALL EliminarUsuario(@id)", comando => AgregarParametro(comando, "@id", id));
        }

        private List<Usuario> ConsultarUsuarios(string sql)
        {
            using var conexion = _crearConexion();
            using var comando = CrearComando(conexion, sql);
            var usuarios = new List<Usuario>();

            conexion.Open();
            using var lector = comando.ExecuteReader();

            while (lector.Read())
            {
                usuarios.Add(MapearUsuario(lector));
            }

            return usuarios;
        }

        private static Usuario MapearUsuario(IDataRecord fila)
        {
            return new Usuario(
                Convert.ToInt32(fila["Id_Usuario"]),
                Convert.ToString(fila["Nombre_Usuario"]) ?? string.Empty,
                Convert.ToString(fila["Genero_Usuario"]) ?? string.Empty,
                Convert.ToString(fila["Email_Usuario"]) ?? string.Empty,
                Convert.ToBoolean(fila["Activo"]),
                fila["Fecha_Eliminacion"] == DBNull.Value ? null : Convert.ToDateTime(fila["Fecha_Eliminacion"]));
        }

        private void Ejecutar(string sql, Action<IDbCommand> configurar)
        {
            using var conexion = _crearConexion();
            using var comando = CrearComando(conexion, sql);
            configurar(comando);

            conexion.Open();
            comando.ExecuteNonQuery();
        }

        private static IDbCommand CrearComando(IDbConnection conexion, string sql)
        {
            var comando = conexion.CreateCommand();
            comando.CommandText = sql;
            return comando;
        }

        private static void AgregarParametro(IDbCommand comando, string nombre, object? valor)
        {
            var parametro = comando.CreateParameter();
            parametro.ParameterName = nombre;
            parametro.Value = valor ?? DBNull.Value;
            comando.Parameters.Add(parametro);
        }
    }
}
