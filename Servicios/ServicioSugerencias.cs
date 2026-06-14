using Simulacion_de_red_social_utilizando_grafos.Clases;

namespace Simulacion_de_red_social_utilizando_grafos.Servicios
{
    public class ServicioSugerencias
    {
        public List<Usuario> ObtenerSugerencias(Grafo grafo, Usuario usuario, int limite = 10)
        {
            return grafo.SugerirAmigos(usuario)
                .Take(limite)
                .ToList();
        }

        public List<Usuario> RecorridoBFS(Grafo grafo, Usuario inicio)
        {
            return grafo.BFS(inicio);
        }

        public List<Usuario> RecorridoDFS(Grafo grafo, Usuario inicio)
        {
            return grafo.DFS(inicio);
        }
    }
}
