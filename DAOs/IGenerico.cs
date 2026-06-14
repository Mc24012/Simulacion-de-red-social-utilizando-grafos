namespace Simulacion_de_red_social_utilizando_grafos.DAOs
{
    public interface IGenerico<T>
    {
        List<T> ObtenerTodos();
        T? ObtenerPorId(int id);
        void Insertar(T entidad);
        void Actualizar(T entidad);
        void Eliminar(int id);
    }
}
