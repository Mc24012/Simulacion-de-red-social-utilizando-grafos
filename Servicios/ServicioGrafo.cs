using System.Diagnostics;
using Simulacion_de_red_social_utilizando_grafos.Clases;
using Simulacion_de_red_social_utilizando_grafos.DAOs;
using Simulacion_de_red_social_utilizando_grafos.Modelos;

namespace Simulacion_de_red_social_utilizando_grafos.Servicios
{
    public class ServicioGrafo
    {
        private readonly UsuarioDAO _usuarioDAO;
        private readonly RelacionDAO _relacionDAO;

        public ServicioGrafo(UsuarioDAO usuarioDAO, RelacionDAO relacionDAO)
        {
            _usuarioDAO = usuarioDAO;
            _relacionDAO = relacionDAO;
        }

        public GrafoSocial CargarGrafoSocial()
        {
            var grafoSocial = new GrafoSocial();

            foreach (var usuario in _usuarioDAO.ObtenerTodos())
            {
                grafoSocial.AgregarUsuario(usuario);
            }

            foreach (var relacion in _relacionDAO.ObtenerTodas())
            {
                if (grafoSocial.ObtenerUsuario(relacion.Id_Seguidor) is not null &&
                    grafoSocial.ObtenerUsuario(relacion.Id_Seguido) is not null)
                {
                    grafoSocial.AgregarRelacion(relacion);
                }
            }

            return grafoSocial;
        }

        public Grafo CargarGrafoManual()
        {
            return CargarGrafoSocial().ConvertirAGrafoManual();
        }

        public List<(string Operacion, long TiempoMs, int ElementosProcesados)> AnalizarRendimiento(Grafo grafo, Clases.Usuario usuarioInicial)
        {
            return
            [
                Medir("BFS", () => grafo.BFS(usuarioInicial).Count),
                Medir("DFS", () => grafo.DFS(usuarioInicial).Count),
                Medir("Sugerencias", () => grafo.SugerirAmigos(usuarioInicial).Count),
                Medir("Exportar GraphX", () =>
                {
                    var graphData = grafo.ExportarParaVisualizacion();
                    return graphData.VertexCount + graphData.EdgeCount;
                })
            ];
        }

        private static (string Operacion, long TiempoMs, int ElementosProcesados) Medir(string operacion, Func<int> ejecutarOperacion)
        {
            var reloj = Stopwatch.StartNew();
            var elementosProcesados = ejecutarOperacion();
            reloj.Stop();

            return (operacion, reloj.ElapsedMilliseconds, elementosProcesados);
        }
    }
}
