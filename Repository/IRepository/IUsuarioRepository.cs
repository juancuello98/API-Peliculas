using ApiPeliculas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Repository.IRepository
{
    public interface IUsuarioRepository
    {
        //se podria crear repositorios por aparte para el registro y para el login, en este caso es todo en este
        ICollection<Usuario> GetUsuarios();

       
        Usuario GetUsuario(int usuarioId);

        bool ExisteUsuario(string nombreUsuario);

        Usuario Registro(Usuario usuario, string password);
        Usuario Login(string nombreUsuario, string password);

        bool Guardar();
    }
}
