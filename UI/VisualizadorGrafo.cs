using GraphX.Common.Enums;
using GraphX.Logic.Models;
using QuickGraph;
using Simulacion_de_red_social_utilizando_grafos.Clases;

namespace Simulacion_de_red_social_utilizando_grafos.UI
{
    public class VisualizadorGrafo
    {
        public GXLogicCore<Usuario, Relacion, BidirectionalGraph<Usuario, Relacion>> VisualizarGrafo(Grafo grafoManual)
        {
            var graphData = grafoManual.ExportarParaVisualizacion();

            var logicCore = new GXLogicCore<Usuario, Relacion, BidirectionalGraph<Usuario, Relacion>>
            {
                Graph = graphData,
                DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.FR
            };

            return logicCore;
        }
    }
}
