using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiPeliculas.Models;
using ApiPeliculas.Models.DTOs;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers
{
    [Authorize]
    [Route("api/[controller]")] // puedo quitar [controller] y poner directamente el nombre de la categoria
    [ApiController]
    [ApiExplorerSettings(GroupName = "ApiPeliculasCategorias")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)] //Esto es generar un codigo de error general para toda la clase, por ej para cuando no se encuentra la ruta 
    public class CategoriasController : Controller // O tambien puede heredar de la clase BaseController , cual es la diferencia?
    {
        // creamos unas propiedas y un constructor
        // instanciamos el mapper

        private readonly IMapper _mapper;

        private readonly ICategoriaRepository _categoriaRepository; // A traves de este repositorio es donde vamos a utilizar los metodos, lo vamos a implementar

        // Instanciamos a ICategoriaRepositor
        public CategoriasController(ICategoriaRepository ctRepo, IMapper mapper)
        {
            _categoriaRepository = ctRepo; // para poder usarlo en toda nuestra aplicacion
            _mapper = mapper;
        }

        //Con esto de abajo agregamos descripcion a Swagger
        /// <summary>
        /// Obtener todas las categorias
        /// </summary>
        /// <returns></returns>

        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200,Type = typeof(List<CategoriaDTO>))] //Aca decimos cuales son los codigos que va a devolver este metodo get
        [ProducesResponseType(400)]
        public IActionResult GetCategorias() // Que es ActionResult?
        {
            var listaCategorias = _categoriaRepository.GetCategorias();
            var listaCategoriaDTO = new List<CategoriaDTO>();
            foreach (var lista in listaCategorias)
            {
                // Usamos el mapper que instanciamos arriba
                listaCategoriaDTO.Add(_mapper.Map<CategoriaDTO>(lista));
            }


            return Ok(listaCategoriaDTO);
        }

        /// <summary>
        /// Obtener una categoria individual
        /// </summary>
        /// <param name="categoriaId">Este es el id de la categoria, el cual se va a buscar</param>
        /// <returns></returns>
        
        [AllowAnonymous]
        [HttpGet("{categoriaId:int}", Name = "GetCategoria")] //parametro para el metodo httpget
        [ProducesResponseType(200, Type = typeof(CategoriaDTO))] //Aca decimos cuales son los codigos que va a devolver este metodo get
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetCategoria(int categoriaId)
        {
            var itemCategoria = _categoriaRepository.GetCategoria(categoriaId);

            if (itemCategoria == null)
            {
                return NotFound();
            }

            var itemCategoriaDTO = _mapper.Map<CategoriaDTO>(itemCategoria);
            return Ok(itemCategoriaDTO); // Devolvemos el DTO para no exponer el modelo categoria, pasamos el categoriaDTO


        }

        /// <summary>
        /// Crea una nueva categoria
        /// </summary>
        /// <param name="categoriaDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(CategoriaDTO))] //Aca decimos cuales son los codigos que va a devolver este metodo get
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // usamos el DTO para no exponer el modelo aca
        // el from body quiere decir que lo que enviemos en el request esta vinculado a lo que obtengamos en el cuerpo
        // de la peticion.
        public IActionResult CrearCategoria([FromBody] CategoriaDTO categoriaDTO)
        {
            if (categoriaDTO == null)
            {
                return BadRequest(ModelState); // ModelState, que es?
            }

            if (_categoriaRepository.ExisteCategoria(categoriaDTO.NombreCategoria))
            {
                ModelState.AddModelError("", "La categoria ya existe");
                return StatusCode(StatusCodes.Status404NotFound, ModelState);
            }

            var categoria = _mapper.Map<Categoria>(categoriaDTO);

            if (!_categoriaRepository.CrearCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal, guardando el registro {categoria.NombreCategoria}");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return CreatedAtRoute("GetCategoria", new { categoriaId = categoria.Id }, categoria); //devuelve el ultimo recurso creado , nos va a devolver con un 201
        }

        /// <summary>
        /// Actualizar una categoria existente
        /// </summary>
        /// <param name="categoriaId"></param>
        /// <param name="categoriaDTO"></param>
        /// <returns></returns>
        [HttpPatch("{categoriaId:int}", Name = "ActualizarCategoria")]
        [ProducesResponseType(204)] //Aca decimos cuales son los codigos que va a devolver este metodo get
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult ActualizarCategoria(int categoriaId, [FromBody] CategoriaDTO categoriaDTO)
        {
            if (categoriaDTO == null || categoriaId != categoriaDTO.Id)
            {
                return BadRequest(ModelState);
            }

            var categoria = _mapper.Map<Categoria>(categoriaDTO);

            if (!_categoriaRepository.ActualizarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal, actualizando el registro {categoria.NombreCategoria}");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return NoContent();
        }

        /// <summary>
        /// Borrar una categoria existente
        /// </summary>
        /// <param name="categoriaId"></param>
        /// <returns></returns>
        [HttpDelete("{categoriaId:int}", Name = "BorrarCategoria")]

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult BorrarCategoria(int categoriaId)
        {
            

            if (!_categoriaRepository.ExisteCategoria(categoriaId))
            {
                return NotFound();
            }

            var categoria = _categoriaRepository.GetCategoria(categoriaId);

            if (!_categoriaRepository.BorrarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal borrando el registro {categoria.NombreCategoria}");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return NoContent();
        }
    }
}
