using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static ApiPeliculas.Models.Pelicula;

namespace ApiPeliculas.Models.DTOs
{
    public class PeliculaUpdateDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }


        [Required(ErrorMessage = "La descripcion es obligatorio")]
        public string Descripcion { get; set; }


        [Required(ErrorMessage = "La duracion es obligatoria")]
        public string Duracion { get; set; }

        public TipoDeClasificacion Clasificacion { get; set; }



        //Estos campos me relacionan la tabla categorias con peliculas
        public int categoriaId { get; set; }
        //public Categoria Categoria { get; set; } esto aca se borra, porque?
    }
}
