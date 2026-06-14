using GraphX.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulacion_de_red_social_utilizando_grafos.Clases
{
    public class Usuario : VertexBase, IEquatable<Usuario>
    {
        public int Id_Usuario
        {
            get => (int)ID;
            set => ID = value;
        }

        public string Nombre_Usuario { get; set; }
        public string Genero_Usuario { get; set; }
        public string Email_Usuario { get; set; }

        public Usuario(int id, string nombre, string genero, string email)
        {
            Id_Usuario = id;
            Nombre_Usuario = nombre;
            Genero_Usuario = genero;
            Email_Usuario = email;
        }

        public bool Equals(Usuario? other)
        {
            if (other is null)
                return false;

            return Id_Usuario == other.Id_Usuario;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Usuario);
        }

        public override int GetHashCode()
        {
            return Id_Usuario.GetHashCode();
        }

        public override string ToString()
        {
            return Nombre_Usuario;
        }
    }
}