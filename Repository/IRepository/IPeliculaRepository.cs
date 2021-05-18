using ApiPeliculas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Repository.IRepository
{
    public interface IPeliculaRepository
    {
        ICollection<Pelicula> GetPeliculas();

        ICollection<Pelicula> GetPeliculasEnCategoria(int categoriaId);
        Pelicula GetPelicula(int peliculaId);

        bool ExistePelicula(string nombre);
        bool ExistePelicula(int peliculaId);

        IEnumerable<Pelicula> BuscarPelicula(string nombre);

        bool CrearPelicula(Pelicula pelicula);
        bool ActualizarPelicula(Pelicula pelicula);

        bool BorrarPelicula(Pelicula pelicula);

        bool Guardar();
    }
}
