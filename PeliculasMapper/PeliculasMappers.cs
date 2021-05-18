using ApiPeliculas.Models;
using ApiPeliculas.Models.DTOs;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.PeliculasMapper
{
    // Aca le estamos diciendo que herede de la clase Profile de Automapper.
    // Aca es donde vinculamos al DTO CategoriaDTO con el modelo Categoria.

    public class PeliculasMappers : Profile
    {
        public PeliculasMappers()
        {
            // Le pasamos  a CreateMap el nombre del modelo principal, y el nombre del DTO.
            // El ReverseMap se usa para indicar que vamos a pasarle de categoriaDTO ---> Categoria y de Categoria ---> CategoriaDTO
            // Categoria <---> CategoriaDTO

            CreateMap<Categoria, CategoriaDTO>().ReverseMap();

            //Map de peliculas
            CreateMap<Pelicula, PeliculaDTO>().ReverseMap();
            CreateMap<Pelicula, PeliculaCreateDTO>().ReverseMap();
            CreateMap<Pelicula, PeliculaUpdateDTO>().ReverseMap();

            CreateMap<Usuario, UsuarioDTO>().ReverseMap();

        }
    }
}
