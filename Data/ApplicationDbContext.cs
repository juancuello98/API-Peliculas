using ApiPeliculas.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Aca creamos nuestro constructor, es una configuracion que es asi por defecto
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        // Este archivo es importante porque cada uno de los modelos 
        // vienen a mapearse aca , como hacemos con el modelo categoria
        // podemos poner cualquier nombre despues del dbSet pero es buena practica
        // poner el mismo nombre, categoria se instancia en categoria
        public DbSet<Categoria> Categoria { get; set; }

        // Esto del DbSet se tiene que hacer siempre antes del add-migration y el update-database
        public DbSet<Pelicula> Pelicula { get; set; }

        public DbSet<Usuario> Usuario { get; set; }


    }
}
