using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }
        public string NombreCategoria { get; set; }

        public DateTime FechaCreacion { get; set; }
    }
}
