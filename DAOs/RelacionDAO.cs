using System.Data;
using Simulacion_de_red_social_utilizando_grafos.Modelos;

namespace Simulacion_de_red_social_utilizando_grafos.DAOs
{
    public class RelacionDAO
    {
        private readonly Func<IDbConnection> _crearConexion;

        public RelacionDAO(Func<IDbConnection> crearConexion)
        {
            _crearConexion = crearConexion;
        }

        public List<Relacion> ObtenerTodas()
        {
            const string sql = """
                SELECT
                    r.Id_Seguidor,
                    r.Id_Seguido,
                    r.FechaInicio,
                    seguidor.Nombre_Usuario AS Nombre_Seguidor,
                    seguidor.Genero_Usuario AS Genero_Seguidor,
                    seguidor.Email_Usuario AS Email_Seguidor,
                    seguidor.Activo AS Activo_Seguidor,
                    seguidor.Fecha_Eliminacion AS Fecha_Eliminacion_Seguidor,
                    seguido.Nombre_Usuario AS Nombre_Seguido,
                    seguido.Genero_Usuario AS Genero_Seguido,
                    seguido.Email_Usuario AS Email_Seguido,
                    seguido.Activo AS Activo_Seguido,
                    seguido.Fecha_Eliminacion AS Fecha_Eliminacion_Seguido
                FROM relaciones r
                INNER JOIN usuarios seguidor ON seguidor.Id_Usuario = r.Id_Seguidor
                INNER JOIN usuarios seguido ON seguido.Id_Usuario = r.Id_Seguido
                """;

            using var conexion = _crearConexion();
            using var comando = conexion.CreateCommand();
            comando.CommandText = sql;
            var relaciones = new List<Relacion>();

            conexion.Open();
            using var lector = comando.ExecuteReader();

            while (lector.Read())
            {
                relaciones.Add(MapearRelacion(lector));
            }

            return relaciones;
        }

        public List<Relacion> ObtenerPorUsuario(int idUsuario)
        {
            const string sql = """
        SELECT
            r.Id_Seguidor,
            r.Id_Seguido,
            r.FechaInicio,
            seguidor.Nombre_Usuario AS Nombre_Seguidor,
            seguidor.Genero_Usuario AS Genero_Seguidor,
            seguidor.Email_Usuario AS Email_Seguidor,
            seguidor.Activo AS Activo_Seguidor,
            seguidor.Fecha_Eliminacion AS Fecha_Eliminacion_Seguidor,
            seguido.Nombre_Usuario AS Nombre_Seguido,
            seguido.Genero_Usuario AS Genero_Seguido,
            seguido.Email_Usuario AS Email_Seguido,
            seguido.Activo AS Activo_Seguido,
            seguido.Fecha_Eliminacion AS Fecha_Eliminacion_Seguido
        FROM relaciones r
        INNER JOIN usuarios seguidor
            ON seguidor.Id_Usuario = r.Id_Seguidor
        INNER JOIN usuarios seguido
            ON seguido.Id_Usuario = r.Id_Seguido
        WHERE r.Id_Seguidor = @idUsuario
           OR r.Id_Seguido = @idUsuario
        """;

            using var conexion = _crearConexion();
            using var comando = conexion.CreateCommand();

            comando.CommandText = sql;

            AgregarParametro(
                comando,
                "@idUsuario",
                idUsuario);

            conexion.Open();

            using var lector = comando.ExecuteReader();

            var relaciones = new List<Relacion>();

            while (lector.Read())
            {
                relaciones.Add(MapearRelacion(lector));
            }

            return relaciones;
        }

        public void Insertar(Relacion relacion)
        {
            const string sql = """
                INSERT INTO relaciones (Id_Seguidor, Id_Seguido, FechaInicio)
                VALUES (@idSeguidor, @idSeguido, @fechaInicio)
                """;

            Ejecutar(sql, comando =>
            {
                AgregarParametro(comando, "@idSeguidor", relacion.Id_Seguidor);
                AgregarParametro(comando, "@idSeguido", relacion.Id_Seguido);
                AgregarParametro(comando, "@fechaInicio", relacion.FechaInicio);
            });
        }

        public void Eliminar(int idSeguidor, int idSeguido)
        {
            const string sql = "DELETE FROM relaciones WHERE Id_Seguidor = @idSeguidor AND Id_Seguido = @idSeguido";

            Ejecutar(sql, comando =>
            {
                AgregarParametro(comando, "@idSeguidor", idSeguidor);
                AgregarParametro(comando, "@idSeguido", idSeguido);
            });
        }

        private static Relacion MapearRelacion(IDataRecord fila)
        {
            var seguidor = new Usuario(
                Convert.ToInt32(fila["Id_Seguidor"]),
                Convert.ToString(fila["Nombre_Seguidor"]) ?? string.Empty,
                Convert.ToString(fila["Genero_Seguidor"]) ?? string.Empty,
                Convert.ToString(fila["Email_Seguidor"]) ?? string.Empty,
                Convert.ToBoolean(fila["Activo_Seguidor"]),
                fila["Fecha_Eliminacion_Seguidor"] == DBNull.Value ? null : Convert.ToDateTime(fila["Fecha_Eliminacion_Seguidor"]));

            var seguido = new Usuario(
                Convert.ToInt32(fila["Id_Seguido"]),
                Convert.ToString(fila["Nombre_Seguido"]) ?? string.Empty,
                Convert.ToString(fila["Genero_Seguido"]) ?? string.Empty,
                Convert.ToString(fila["Email_Seguido"]) ?? string.Empty,
                Convert.ToBoolean(fila["Activo_Seguido"]),
                fila["Fecha_Eliminacion_Seguido"] == DBNull.Value ? null : Convert.ToDateTime(fila["Fecha_Eliminacion_Seguido"]));

            return new Relacion(seguidor, seguido, Convert.ToDateTime(fila["FechaInicio"]));
        }

        private void Ejecutar(string sql, Action<IDbCommand> configurar)
        {
            using var conexion = _crearConexion();
            using var comando = conexion.CreateCommand();
            comando.CommandText = sql;
            configurar(comando);

            conexion.Open();
            comando.ExecuteNonQuery();
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
