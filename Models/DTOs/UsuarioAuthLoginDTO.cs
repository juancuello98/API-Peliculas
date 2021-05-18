using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Models.DTOs
{
    public class UsuarioAuthLoginDTO
    {
        [Required(ErrorMessage = "El usuario es obligatorio")]
        public string Usuario { get; set; } // Usuario que me sirve para el registro, es obligatorio

        [Required(ErrorMessage = "La contrasenia es obligatorio")]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "La contrasenia debe estar entre 4 y 10 caracteres")]
        public string Password { get; set; } // Lo recbimos como string y interiormente le creamos el hash y el salt
    }
}
