namespace Simulacion_de_red_social_utilizando_grafos.Modelos
{
    public class Relacion : Clases.Relacion
    {
        public int Id_Seguidor => Seguidor.Id_Usuario;
        public int Id_Seguido => Seguido.Id_Usuario;

        public Relacion(Usuario seguidor, Usuario seguido, DateTime fechaInicio)
            : base(seguidor, seguido)
        {
            FechaInicio = fechaInicio;
        }
    }
}
