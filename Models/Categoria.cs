using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class Categoria
    {
        [Key]
        public int Id { get; set; }
        public string NombreCategoria { get; set; }

        public DateTime FechaCreacion { get; set; }
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
