using System.Data;
using MySqlConnector;

namespace Simulacion_de_red_social_utilizando_grafos.BaseDatos
{
    public static class Conexion
    {
        public const string CadenaConexion =
            "Server=127.0.0.1;Port=3306;Database=red_db;User ID=root;Password=;";

        public static IDbConnection CrearConexion()
        {
            return new MySqlConnection(CadenaConexion);
        }
    }
}
