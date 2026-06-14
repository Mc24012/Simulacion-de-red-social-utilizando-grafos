using GraphX.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulacion_de_red_social_utilizando_grafos.Clases
{
    public class Relacion: EdgeBase<Usuario>
    {
        
        public Usuario Seguidor 
        {
            get => Source;
            set => Source = value;
        }
        public Usuario Seguido
        {
            get => Target;
            set => Target = value;
        }
        public DateTime FechaInicio { get; set; }
        public Relacion(Usuario seguidor, Usuario seguido) : base(seguidor, seguido)
        {

        }
    }
}
