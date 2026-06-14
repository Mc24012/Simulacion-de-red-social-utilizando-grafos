using Simulacion_de_red_social_utilizando_grafos.Clases;

namespace Simulacion_de_red_social_utilizando_grafos.Modelos
{
    public class GrafoSocial
    {
        private readonly Dictionary<int, Usuario> _usuarios = [];
        private readonly List<Relacion> _relaciones = [];

        public IReadOnlyCollection<Usuario> Usuarios => _usuarios.Values;
        public IReadOnlyCollection<Relacion> Relaciones => _relaciones;

        public void AgregarUsuario(Usuario usuario)
        {
            _usuarios[usuario.Id_Usuario] = usuario;
        }

        public void AgregarRelacion(Relacion relacion)
        {
            if (!_usuarios.ContainsKey(relacion.Id_Seguidor))
            {
                AgregarUsuario((Usuario)relacion.Seguidor);
            }

            if (!_usuarios.ContainsKey(relacion.Id_Seguido))
            {
                AgregarUsuario((Usuario)relacion.Seguido);
            }

            _relaciones.Add(relacion);
        }

        public Usuario? ObtenerUsuario(int idUsuario)
        {
            return _usuarios.TryGetValue(idUsuario, out var usuario) ? usuario : null;
        }

        public Grafo ConvertirAGrafoManual()
        {
            var grafo = new Grafo();

            foreach (var usuario in Usuarios)
            {
                grafo.AgregarUsuario(usuario);
            }

            foreach (var relacion in Relaciones)
            {
                grafo.Seguir(relacion.Seguidor, relacion.Seguido);
            }

            return grafo;
        }
    }
}
