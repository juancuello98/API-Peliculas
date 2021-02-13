using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Repository
{
    public class CategoriaRepository : ICategoriaRepository
    {
        // Instanciamos la cadena de conexion

        private readonly ApplicationDbContext _dataBase;

        public CategoriaRepository(ApplicationDbContext dataBase)
        {
            _dataBase = dataBase;
        }
        public bool ActualizarCategoria(Categoria categoria)
        {
            _dataBase.Categoria.Update(categoria);
            return Guardar();
        }

        public bool BorrarCategoria(Categoria categoria)
        {

            _dataBase.Categoria.Remove(categoria);
            return Guardar();
        }

        public bool CrearCategoria(Categoria categoria)
        {

            _dataBase.Categoria.Add(categoria);
            return Guardar();
        }

        public bool ExisteCategoria(string nombre)
        {
            /* Creamos una variable booleana, accedemos al contexto por Categoria, con Any (esto es de EF) busco  si existe 
             usamos expresiones regulares , ToLower() -> lo pasa a minuscula, Trim() remueve los espacios en blanco*/

            bool valor = _dataBase.Categoria.Any(c => c.NombreCategoria.ToLower().Trim() == nombre.ToLower().Trim());
            return valor;
        }

        public bool ExisteCategoria(int CategoriaId)
        {
            bool valor = _dataBase.Categoria.Any(c => c.Id == CategoriaId);
            return valor;
        }

        public Categoria GetCategoria(int CategoriaId)
        {
            return _dataBase.Categoria.FirstOrDefault(c => c.Id == CategoriaId);
        }

        public ICollection<Categoria> GetCategorias()
        {
            return _dataBase.Categoria.OrderBy(c => c.NombreCategoria).ToList();
            // para orden descendente OrderByDescending
        }

        public bool Guardar()
        {
            // lo llamamos siempre a este metodo despues de una consulta a la bdd, si por alguna razon falla la insercion
            // de una tabla por ejemplo, va a retornar un false.

            return _dataBase.SaveChanges() >= 0 ? true : false;
        }
    }
}
