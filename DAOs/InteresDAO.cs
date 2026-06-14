using System.Data;
using System.Windows;

namespace Simulacion_de_red_social_utilizando_grafos.DAOs
{
    public class InteresDAO
    {
        private readonly Func<IDbConnection> _crearConexion;

        private readonly string[] _interesesBase =
        [
            "Musica", "Cine", "Deportes", "Lectura", "Videojuegos", "Tecnologia",
            "Viajes", "Cocina", "Arte", "Fotografia", "Fitness", "Programacion"
        ];

        public InteresDAO(Func<IDbConnection> crearConexion)
        {
            _crearConexion = crearConexion;
        }

        public void AsegurarTablasYDatos()
        {
            Ejecutar("""
                CREATE TABLE IF NOT EXISTS intereses (
                    Id_Interes INT NOT NULL AUTO_INCREMENT,
                    Nombre_Interes VARCHAR(80) NOT NULL,
                    PRIMARY KEY (Id_Interes),
                    UNIQUE KEY uq_nombre_interes (Nombre_Interes)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
                """);

            Ejecutar("""
                CREATE TABLE IF NOT EXISTS usuario_intereses (
                    Id_Usuario INT NOT NULL,
                    Id_Interes INT NOT NULL,
                    PRIMARY KEY (Id_Usuario, Id_Interes),
                    CONSTRAINT fk_usuario_interes_usuario
                        FOREIGN KEY (Id_Usuario) REFERENCES usuarios (Id_Usuario)
                        ON DELETE CASCADE,
                    CONSTRAINT fk_usuario_interes_interes
                        FOREIGN KEY (Id_Interes) REFERENCES intereses (Id_Interes)
                        ON DELETE CASCADE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
                """);

            Ejecutar("""
                CREATE TABLE IF NOT EXISTS posibles_relaciones (
                    Id_Origen INT NOT NULL,
                    Id_Destino INT NOT NULL,
                    Interes_Comun VARCHAR(80) NOT NULL,
                    Puntaje INT NOT NULL DEFAULT 1,
                    Fecha_Calculo DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    PRIMARY KEY (Id_Origen, Id_Destino, Interes_Comun),
                    CONSTRAINT fk_posible_origen
                        FOREIGN KEY (Id_Origen) REFERENCES usuarios (Id_Usuario)
                        ON DELETE CASCADE,
                    CONSTRAINT fk_posible_destino
                        FOREIGN KEY (Id_Destino) REFERENCES usuarios (Id_Usuario)
                        ON DELETE CASCADE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
                """);

            foreach (var interes in _interesesBase)
            {
                Ejecutar("INSERT IGNORE INTO intereses (Nombre_Interes) VALUES (@nombre)", comando =>
                {
                    AgregarParametro(comando, "@nombre", interes);
                });
            }

            AsignarInteresesIniciales();
        }

        public Dictionary<int, List<string>> ObtenerInteresesPorUsuario()
        {
            const string sql = """
                SELECT ui.Id_Usuario, i.Nombre_Interes
                FROM usuario_intereses ui
                INNER JOIN intereses i ON i.Id_Interes = ui.Id_Interes
                ORDER BY ui.Id_Usuario, i.Nombre_Interes
                """;

            var resultado = new Dictionary<int, List<string>>();

            using var conexion = _crearConexion();
            using var comando = conexion.CreateCommand();
            comando.CommandText = sql;

            conexion.Open();
            using var lector = comando.ExecuteReader();

            while (lector.Read())
            {
                var idUsuario = Convert.ToInt32(lector["Id_Usuario"]);
                var interes = Convert.ToString(lector["Nombre_Interes"]) ?? string.Empty;

                if (!resultado.ContainsKey(idUsuario))
                {
                    resultado[idUsuario] = [];
                }

                resultado[idUsuario].Add(interes);
            }

            return resultado;
        }

        public List<(int Id, string Nombre)> ObtenerTodos()
        {
            return ObtenerIntereses();
        }

        public HashSet<int> ObtenerIdsPorUsuario(int idUsuario)
        {
            const string sql = "SELECT Id_Interes FROM usuario_intereses WHERE Id_Usuario = @idUsuario";
            var intereses = new HashSet<int>();

            using var conexion = _crearConexion();
            using var comando = conexion.CreateCommand();
            comando.CommandText = sql;
            AgregarParametro(comando, "@idUsuario", idUsuario);

            conexion.Open();
            using var lector = comando.ExecuteReader();

            while (lector.Read())
            {
                intereses.Add(Convert.ToInt32(lector["Id_Interes"]));
            }

            return intereses;
        }

        public void GuardarInteresesUsuario(int idUsuario, IEnumerable<int> idsIntereses)
        {
            Ejecutar("DELETE FROM usuario_intereses WHERE Id_Usuario = @idUsuario", comando =>
            {
                AgregarParametro(comando, "@idUsuario", idUsuario);
            });

            foreach (var idInteres in idsIntereses.Distinct())
            {
                Ejecutar("INSERT INTO usuario_intereses (Id_Usuario, Id_Interes) VALUES (@idUsuario, @idInteres)", comando =>
                {
                    AgregarParametro(comando, "@idUsuario", idUsuario);
                    AgregarParametro(comando, "@idInteres", idInteres);
                });
            }
        }

        public HashSet<(int Origen, int Destino, string Interes)> ObtenerPosiblesRelacionesPorIntereses(HashSet<(int Seguidor, int Seguido)> relacionesExistentes)
        {
            var intereses = ObtenerInteresesPorUsuario();
            var usuarios = intereses.Keys.OrderBy(id => id).ToList();
            var resultado = new HashSet<(int Origen, int Destino, string Interes)>();

            foreach (var usuario in usuarios)
            {
                var candidatos = usuarios
                    .Where(candidato => candidato != usuario)
                    .Where(candidato => !relacionesExistentes.Contains((usuario, candidato)))
                    .Select(candidato => new
                    {
                        Id = candidato,
                        Comunes = intereses[usuario].Intersect(intereses[candidato]).ToList()
                    })
                    .Where(x => x.Comunes.Count > 0)
                    .OrderByDescending(x => x.Comunes.Count)
                    .ThenBy(x => x.Id)
                    .Take(2);

                foreach (var candidato in candidatos)
                {
                    var origen = Math.Min(usuario, candidato.Id);
                    var destino = Math.Max(usuario, candidato.Id);
                    resultado.Add((origen, destino, candidato.Comunes[0]));
                }
            }

            return resultado;
        }

        public void GuardarPosiblesRelaciones(IEnumerable<(int Origen, int Destino, string Interes)> posiblesRelaciones)
        {
            Ejecutar("DELETE FROM posibles_relaciones");

            foreach (var relacion in posiblesRelaciones)
            {
                Ejecutar("""
                    INSERT INTO posibles_relaciones (Id_Origen, Id_Destino, Interes_Comun, Puntaje, Fecha_Calculo)
                    VALUES (@origen, @destino, @interes, @puntaje, NOW())
                    """, comando =>
                {
                    AgregarParametro(comando, "@origen", relacion.Origen);
                    AgregarParametro(comando, "@destino", relacion.Destino);
                    AgregarParametro(comando, "@interes", relacion.Interes);
                    AgregarParametro(comando, "@puntaje", 1);
                });
            }
        }

        private void AsignarInteresesIniciales()
        {
            const string sqlUsuarios = """
                SELECT Id_Usuario
                FROM usuarios
                WHERE Id_Usuario NOT IN (SELECT DISTINCT Id_Usuario FROM usuario_intereses)
                ORDER BY Id_Usuario
                """;

            var usuariosSinIntereses = new List<int>();
            var intereses = ObtenerIntereses();

            using (var conexion = _crearConexion())
            using (var comando = conexion.CreateCommand())
            {
                comando.CommandText = sqlUsuarios;
                conexion.Open();

                using var lector = comando.ExecuteReader();
                while (lector.Read())
                {
                    usuariosSinIntereses.Add(Convert.ToInt32(lector["Id_Usuario"]));
                }
            }
            if (!intereses.Any())
            {
                MessageBox.Show(
                    "No existen intereses registrados en la base de datos.",
                    "Advertencia",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            foreach (var idUsuario in usuariosSinIntereses)
            {
                var indices = new[]
                {
                    idUsuario % intereses.Count,
                    (idUsuario + 3) % intereses.Count,
                    (idUsuario + 7) % intereses.Count
                };

                foreach (var indice in indices.Distinct())
                {
                    Ejecutar("INSERT IGNORE INTO usuario_intereses (Id_Usuario, Id_Interes) VALUES (@idUsuario, @idInteres)", comando =>
                    {
                        AgregarParametro(comando, "@idUsuario", idUsuario);
                        AgregarParametro(comando, "@idInteres", intereses[indice].Id);
                    });
                }
            }
        }

        private List<(int Id, string Nombre)> ObtenerIntereses()
        {
            const string sql = "SELECT Id_Interes, Nombre_Interes FROM intereses ORDER BY Id_Interes";
            var intereses = new List<(int Id, string Nombre)>();

            using var conexion = _crearConexion();
            using var comando = conexion.CreateCommand();
            comando.CommandText = sql;

            conexion.Open();
            using var lector = comando.ExecuteReader();

            while (lector.Read())
            {
                intereses.Add((Convert.ToInt32(lector["Id_Interes"]), Convert.ToString(lector["Nombre_Interes"]) ?? string.Empty));
            }

            return intereses;
        }

        private void Ejecutar(string sql, Action<IDbCommand>? configurar = null)
        {
            using var conexion = _crearConexion();
            using var comando = conexion.CreateCommand();
            comando.CommandText = sql;
            configurar?.Invoke(comando);

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
