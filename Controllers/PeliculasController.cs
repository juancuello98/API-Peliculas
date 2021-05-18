using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApiPeliculas.Models;
using ApiPeliculas.Models.DTOs;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers
{
    [Authorize]
    [Route("api/[controller]")] // puedo quitar [controller] y poner directamente el nombre de la Pelicula
    [ApiController]
    [ApiExplorerSettings(GroupName = "ApiPeliculas")] // Esto es para que nos salga todos estos documentos en su apartado correspondiente de swagger
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class PeliculasController : Controller // O tambien puede heredar de la clase BaseController , cual es la diferencia?
    {
        // creamos unas propiedas y un constructor
        // instanciamos el mapper

        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostingEnvironment;

        private readonly IPeliculaRepository _peliculaRepository; // A traves de este repositorio es donde vamos a utilizar los metodos, lo vamos a implementar

        // Instanciamos a IPeliculaRepositor
        public PeliculasController(IPeliculaRepository pRepo, IMapper mapper, IWebHostEnvironment hostingEnvironment)
        {
            _peliculaRepository = pRepo; // para poder usarlo en toda nuestra aplicacion
            _mapper = mapper;
            _hostingEnvironment = hostingEnvironment;
    }

        /// <summary>
        /// Obtener todas las peliculas
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<PeliculaDTO>))] //Aca decimos cuales son los codigos que va a devolver este metodo get
        [ProducesResponseType(400)]
        public IActionResult GetPeliculas() // Que es ActionResult?
        {
            var listaPeliculas = _peliculaRepository.GetPeliculas();
            var listaPeliculaDTO = new List<PeliculaDTO>();
            foreach (var lista in listaPeliculas)
            {
                // Usamos el mapper que instanciamos arriba
                listaPeliculaDTO.Add(_mapper.Map<PeliculaDTO>(lista));
            }


            return Ok(listaPeliculaDTO);
        }

        /// <summary>
        /// Obtener todas las peliculas de una categoria
        /// </summary>
        /// <param name="categoriaId">Id de referencia de la categoria</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("GetPeliculasEnCategoria/{categoriaId:int}", Name = "GetPeliculasEnCategoria")]
        [ProducesResponseType(200, Type = typeof(List<PeliculaDTO>))] 
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetPeliculasEnCategoria(int categoriaId) // Que es ActionResult?
        {
            var listaPeliculas = _peliculaRepository.GetPeliculasEnCategoria(categoriaId);

            if(listaPeliculas == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, ModelState);
            }

            var listaPeliculaDTO = new List<PeliculaDTO>();

            foreach (var item in listaPeliculas)
            {
                // Usamos el mapper que instanciamos arriba
                listaPeliculaDTO.Add(_mapper.Map<PeliculaDTO>(item));
            }


            return Ok(listaPeliculaDTO);
        }


        /// <summary>
        /// Obtener una pelicula 
        /// </summary>
        /// <param name="PeliculaId">Id de la pelicula a buscar</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{PeliculaId:int}", Name = "GetPelicula")] //parametro para el metodo httpget
        [ProducesResponseType(200, Type = typeof(PeliculaDTO))] //Aca decimos cuales son los codigos que va a devolver este metodo get
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetPelicula(int PeliculaId)
        {
            var itemPelicula = _peliculaRepository.GetPelicula(PeliculaId);

            if (itemPelicula == null)
            {
                return NotFound();
            }

            var itemPeliculaDTO = _mapper.Map<PeliculaDTO>(itemPelicula);
            return Ok(itemPeliculaDTO); // Devolvemos el DTO para no exponer el modelo Pelicula, pasamos el PeliculaDTO


        }

        /// <summary>
        /// Crea una nueva pelicula
        /// </summary>
        /// <param name="PeliculaDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Pelicula))] 
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // usamos el DTO para no exponer el modelo aca
        // FromForm , obtenemos los datos de un formulario
        public IActionResult CrearPelicula([FromForm] PeliculaCreateDTO PeliculaDTO)
        {
            if (PeliculaDTO == null)
            {
                return BadRequest(ModelState); // ModelState, que es?
            }

            if (_peliculaRepository.ExistePelicula(PeliculaDTO.Nombre))
            {
                ModelState.AddModelError("", "La Pelicula ya existe");
                return StatusCode(StatusCodes.Status404NotFound, ModelState);
            }

            /* **********   Subida de Archivos ****** */

            var archivo = PeliculaDTO.Foto;
            string rutaPrincipal = _hostingEnvironment.WebRootPath; // instancio el hostin enviroment, y instancio la ruta
            var archivos = HttpContext.Request.Form.Files;


            if (archivo.Length > 0)
            {
                //Nueva imagen
                string nombreFoto = Guid.NewGuid().ToString();// el Guid.NewGuid(), es un campo que contiene una secuencia de caracteres irrepetibles que me sirven para anadir al nombre de la imagen
                                                              // y que asi no se reemplacen las imagenes que se suban con un mismo nombre.

                var subidas = Path.Combine(rutaPrincipal, @"fotos");
                var extension = Path.GetExtension(archivos[0].FileName);

                using (var fileStreams = new FileStream(Path.Combine(subidas, nombreFoto + extension),FileMode.Create))
                {
                    archivos[0].CopyTo(fileStreams);
                }

                PeliculaDTO.RutaImagen = @"\fotos\" + nombreFoto + extension;

            }
            /* ********** ****** */



            var Pelicula = _mapper.Map<Pelicula>(PeliculaDTO);

            if (!_peliculaRepository.CrearPelicula(Pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal, guardando el registro {Pelicula.Nombre}");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return CreatedAtRoute("GetPelicula", new { PeliculaId = Pelicula.Id }, Pelicula); //devuelve el ultimo recurso creado , nos va a devolver con un 201
        }


        /// <summary>
        /// Actualizar una pelicula
        /// </summary>
        /// <param name="PeliculaId"></param>
        /// <param name="PeliculaDTO"></param>
        /// <returns></returns>
        [HttpPatch("{PeliculaId:int}", Name = "ActualizarPelicula")]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActualizarPelicula(int PeliculaId, [FromBody] PeliculaUpdateDTO PeliculaDTO)
        {
            if (PeliculaDTO == null || PeliculaId != PeliculaDTO.Id)
            {
                return BadRequest(ModelState);
            }

            var Pelicula = _mapper.Map<Pelicula>(PeliculaDTO);

            if (!_peliculaRepository.ActualizarPelicula(Pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal, actualizando el registro {Pelicula.Nombre}");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return NoContent();
        }

        /// <summary>
        /// Borra una pelicula existente
        /// </summary>
        /// <param name="PeliculaId">Id de la pelicula a borrar</param>
        /// <returns></returns>
        [HttpDelete("{PeliculaId:int}", Name = "BorrarPelicula")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult BorrarPelicula(int PeliculaId)
        {
            

            if (!_peliculaRepository.ExistePelicula(PeliculaId))
            {
                return NotFound();
            }

            var Pelicula = _peliculaRepository.GetPelicula(PeliculaId);

            if (!_peliculaRepository.BorrarPelicula(Pelicula))
            {
                ModelState.AddModelError("", $"Algo salio mal borrando el registro {Pelicula.Nombre}");
                return StatusCode(StatusCodes.Status500InternalServerError, ModelState);
            }

            return NoContent();
        }

        /// <summary>
        /// Busca y devuelve una pelicula existente
        /// </summary>
        /// <param name="nombre">Nombre de la pelicula a buscar</param>
        /// <returns></returns>
        [HttpGet("Buscar")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pelicula>))] //Aca decimos cuales son los codigos que va a devolver este metodo get
        [ProducesResponseType(400)]
        public IActionResult Buscar(string nombre)
        {
            try
            {
                var resultado = _peliculaRepository.BuscarPelicula(nombre);
                if (resultado.Any()) // si en la base de datos se encontro algo, devuelve un Ok
                {
                    return Ok(resultado);
                }

                return NotFound();
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, "Error recuperando datos de la aplicacion");
            }
        }
    }
}
