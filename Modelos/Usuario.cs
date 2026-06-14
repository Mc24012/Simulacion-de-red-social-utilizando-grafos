using Simulacion_de_red_social_utilizando_grafos.Clases;

namespace Simulacion_de_red_social_utilizando_grafos.Modelos
{
    public class Usuario : Clases.Usuario
    {
        public bool Activo { get; set; }
        public DateTime? Fecha_Eliminacion { get; set; }

        public Usuario(int id, string nombre, string genero, string email, bool activo = true, DateTime? fechaEliminacion = null)
            : base(id, nombre, genero, email)
        {
            Activo = activo;
            Fecha_Eliminacion = fechaEliminacion;
        }
    }
}
