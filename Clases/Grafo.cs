using QuickGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulacion_de_red_social_utilizando_grafos.Clases
{
    public class Grafo
    {
        private Dictionary<Usuario, HashSet<Usuario>> _Seguidos; // Este usuario sigue a estos
        private Dictionary<Usuario, HashSet<Usuario>> _Seguidores; // A este usuarios lo siguen estos

        public Grafo()
        {
            _Seguidos = new Dictionary<Usuario, HashSet<Usuario>>();
            _Seguidores = new Dictionary<Usuario, HashSet<Usuario>>();
        }

        public void Seguir(Usuario seguidor, Usuario seguido)
        {
            if (!_Seguidos.ContainsKey(seguidor))
                AgregarUsuario(seguidor);

            if (!_Seguidos.ContainsKey(seguido))
                AgregarUsuario(seguido);

            _Seguidos[seguidor].Add(seguido);
            _Seguidores[seguido].Add(seguidor);
        }

        public void DejarSeguir(Usuario seguidor, Usuario seguido)
        {
            if (!_Seguidos.TryGetValue(seguidor, out var seguidos))
                return;

            if (!_Seguidores.TryGetValue(seguido, out var seguidores))
                return;

            seguidos.Remove(seguido);
            seguidores.Remove(seguidor);
        }

        public List<Usuario> SugerirAmigos(Usuario usuario)
        {
            if (!_Seguidos.TryGetValue(usuario, out var seguidosUsuario))
                return [];

            Dictionary<Usuario, int> candidatos = [];

            foreach (var seguido in seguidosUsuario)
            {
                if (!_Seguidos.TryGetValue(seguido, out var seguidos))
                    continue;

                foreach (var candidato in seguidos)
                {
                    if (candidato == usuario)
                        continue;

                    if (seguidosUsuario.Contains(candidato))
                        continue;

                    candidatos[candidato] =
                        candidatos.GetValueOrDefault(candidato, 0) + 1;
                }
            }

            return candidatos
                .OrderByDescending(x => x.Value)
                .Select(x => x.Key)
                .ToList();
        }

        public void AgregarUsuario(Usuario usuario)
        {
            //inicializamos el usuario nuevo con una lista de amigos vacia
            if (!_Seguidos.ContainsKey(usuario))
                _Seguidos[usuario] = new HashSet<Usuario>();

            if (!_Seguidores.ContainsKey(usuario))
                _Seguidores[usuario] = new HashSet<Usuario>();
        }
        public BidirectionalGraph<Usuario, Relacion> ExportarParaVisualizacion()
        {
            var graph = new BidirectionalGraph<Usuario, Relacion>();

            // Agregar todos los usuarios  
            foreach (var usuario in _Seguidos.Keys)
            {
                graph.AddVertex(usuario);
            }

            // Agregar todas las relaciones  
            foreach (var kvp in _Seguidos)
            {
                var seguidor = kvp.Key;
                foreach (var seguido in kvp.Value)
                {
                    var relacion = new Relacion(seguidor, seguido);
                    graph.AddEdge(relacion);
                }
            }

            return graph;
        }
        public List<Usuario> BFS(Usuario inicio)
        {
            var resultado = new List<Usuario>();

            if (inicio == null)
                return resultado;

            if (!_Seguidos.ContainsKey(inicio))
                return resultado;

            var visitados = new HashSet<Usuario>();
            var cola = new Queue<Usuario>();

            visitados.Add(inicio);
            cola.Enqueue(inicio);

            while (cola.Count > 0)
            {
                var actual = cola.Dequeue();
                resultado.Add(actual);

                if (!_Seguidos.TryGetValue(actual, out var vecinos))
                    continue;

                foreach (var vecino in vecinos)
                {
                    if (visitados.Add(vecino))
                    {
                        cola.Enqueue(vecino);
                    }
                }
            }

            return resultado;
        }

        public List<Usuario> DFS(Usuario inicio)
        {
            var resultado = new List<Usuario>();

            if (inicio == null)
                return resultado;

            if (!_Seguidos.ContainsKey(inicio))
                return resultado;

            DFSRecursivo(inicio, new HashSet<Usuario>(), resultado);

            return resultado;
        }

        private void DFSRecursivo(
     Usuario actual,
     HashSet<Usuario> visitados,
     List<Usuario> resultado)
        {
            visitados.Add(actual);
            resultado.Add(actual);

            if (!_Seguidos.TryGetValue(actual, out var vecinos))
                return;

            foreach (var vecino in vecinos)
            {
                if (!visitados.Contains(vecino))
                {
                    DFSRecursivo(vecino, visitados, resultado);
                }
            }
        }
    }
}