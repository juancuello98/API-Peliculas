using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ApiPeliculas.Models;
using ApiPeliculas.Models.DTOs;
using ApiPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ApiPeliculas.Controllers
{
    [Authorize]
    [Route("api/Usuarios")] // puedo quitar [controller] y poner directamente el nombre de la categoria
    [ApiController]
    [ApiExplorerSettings(GroupName = "ApiPeliculasUsuarios")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class UsuariosController : Controller // O tambien puede heredar de la clase BaseController , cual es la diferencia?
    {
        // creamos unas propiedas y un constructor
        // instanciamos el mapper

        private readonly IMapper _mapper;

        private readonly IUsuarioRepository _userRepository; // A traves de este repositorio es donde vamos a utilizar los metodos, lo vamos a implementar

        private readonly IConfiguration _config; // nueva variable global de tipo privada y readonly, instanciada en _config, esto se pasa al constructor tambien, es de using Microsoft.Extensions.Configuration;
        // Instanciamos a ICategoriaRepositor
        public UsuariosController(IUsuarioRepository userRepo, IMapper mapper, IConfiguration config)
        {
            _userRepository = userRepo; // para poder usarlo en toda nuestra aplicacion
            _mapper = mapper;
            _config = config;
        }

        /// <summary>
        /// Obtener usuarios 
        /// </summary>
        /// <returns></returns>
        [HttpGet (Name = "GetUsuarios")]
        [ProducesResponseType(200, Type = typeof(List<UsuarioDTO>))] 
        [ProducesResponseType(400)]
        public IActionResult GetUsuarios() // Que es ActionResult?
        {
            var listaUsuarios = _userRepository.GetUsuarios();
            var listaUsuariosDTO = new List<UsuarioDTO>();
            foreach (var lista in listaUsuarios)
            {
                // Usamos el mapper que instanciamos arriba
                listaUsuariosDTO.Add(_mapper.Map<UsuarioDTO>(lista));
            }


            return Ok(listaUsuariosDTO);
        }

        /// <summary>
        /// Obtener un usuario
        /// </summary>
        /// <param name="userId">Id del usuario a obtener</param>
        /// <returns></returns>
        [HttpGet("{userId:int}", Name = "GetUsuario")] //parametro para el metodo httpget
        [ProducesResponseType(200, Type = typeof(UsuarioDTO))] //Aca decimos cuales son los codigos que va a devolver este metodo get
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
        public IActionResult GetUsuario(int userId)
        {
            var itemUsuario = _userRepository.GetUsuario(userId);

            if (itemUsuario == null)
            {
                return NotFound();
            }

            var itemUsuarioDTO = _mapper.Map<UsuarioDTO>(itemUsuario);
            return Ok(itemUsuarioDTO); // Devolvemos el DTO para no exponer el modelo categoria, pasamos el categoriaDTO


        }

        /// <summary>
        /// Registrar un nuevo usuario
        /// </summary>
        /// <param name="usuarioAuthDTO"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Registro")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Registro(UsuarioAuthDTO usuarioAuthDTO)
        {
            usuarioAuthDTO.Usuario = usuarioAuthDTO.Usuario.ToLower();
            if (_userRepository.ExisteUsuario(usuarioAuthDTO.Usuario))
            {
                return BadRequest("El usuario ya existe");
            }

            var usuarioACrear = new Usuario
            {
                UsuarioA = usuarioAuthDTO.Usuario

            };

            var usuarioCreado = _userRepository.Registro(usuarioACrear, usuarioAuthDTO.Password);
            return Ok(usuarioCreado);
        }

        /// <summary>
        /// Acceso/Autorizacion usuario
        /// </summary>
        /// <param name="usuarioAuthLoginDTO"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Login")]
        [ProducesResponseType(200, Type = typeof(SecurityToken))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesDefaultResponseType]
        public IActionResult Login(UsuarioAuthLoginDTO usuarioAuthLoginDTO)
        {
            // Implementando el Login con el JWT

            var usuarioDesdeRepo = _userRepository.Login(usuarioAuthLoginDTO.Usuario, usuarioAuthLoginDTO.Password);
            // Primer comprobacion, credenciales de usuario y contrasenia no se encuentran en la bdd
            if (usuarioDesdeRepo == null)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioDesdeRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, usuarioDesdeRepo.UsuarioA.ToString())

            };


            //Empiezo a crear el token que me genera , generacion de token

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value)); // el getSection da un error
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature); // crea las credenciales del token

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),    // Le configuramos la expiracion al token
                SigningCredentials = credenciales
            };
            
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });

            // Aca termina la generacion del token


        
        }


    }
}
