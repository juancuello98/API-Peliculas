using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Repository
{
    public class PeliculaRepository : IPeliculaRepository
    {
        // Instanciamos la cadena de conexion

        private readonly ApplicationDbContext _dataBase;

        public PeliculaRepository(ApplicationDbContext dataBase)
        {
            _dataBase = dataBase;
        }

        public bool ActualizarPelicula(Pelicula pelicula)
        {
            _dataBase.Update(pelicula);
            return Guardar();
        }

        public bool BorrarPelicula(Pelicula pelicula)
        {
            _dataBase.Remove(pelicula);
            return Guardar();
        }

        public IEnumerable<Pelicula> BuscarPelicula(string nombre)
        {
            IQueryable<Pelicula> query = _dataBase.Pelicula; // en esta linea estamos diciendo que instanciamos una variable 
                                                             //query de tipo Iqueryable donde le decimos que se puden hacer consultas
                                                             // a la base de datos sobre la entidad Pelicula.
            if (!string.IsNullOrEmpty(nombre))
            {
                // aca estamos diciendo que entre si no es nulo ni vacio lo que tiene nombre
                query = query.Where(e => e.Nombre.Contains(nombre) || e.Descripcion.Contains(nombre));
            }

            return query.ToList();
        }

        public bool CrearPelicula(Pelicula pelicula)
        {
            _dataBase.Add(pelicula);
            return Guardar();
        }

        public bool ExistePelicula(int peliculaId)
        {
            return _dataBase.Pelicula.Any(p => p.Id == peliculaId);
           
        }

        public bool ExistePelicula(string nombre)
        {
            bool valor = _dataBase.Pelicula.Any(p => p.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            return valor;

        }

        public Pelicula GetPelicula(int peliculaId)
        {
            return _dataBase.Pelicula.FirstOrDefault(p => p.Id == peliculaId);
        }

        public ICollection<Pelicula> GetPeliculas()
        {
            return _dataBase.Pelicula.OrderBy(p => p.Nombre).ToList();
        }

        public ICollection<Pelicula> GetPeliculasEnCategoria(int catId)
        {
            return _dataBase.Pelicula.Include(ca => ca.Categoria).Where(c => c.categoriaId == catId).ToList();
        }

        public bool Guardar()
        {
            return _dataBase.SaveChanges() >= 0 ? true : false; 
        }
    }
}
