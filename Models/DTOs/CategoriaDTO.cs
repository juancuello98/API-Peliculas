using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models.DTOs
{
    public class CategoriaDTO
    {
       
        public int Id { get; set; }

        // Esto lo que hace es que sea obligatorio el nombre, y que salga un error si lo dejan en blanco
        // le agregamos un mensaje personalizado

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string NombreCategoria { get; set; }

        public DateTime FechaCreacion { get; set; }
    }
}
