using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static ApiPeliculas.Models.Pelicula;

namespace ApiPeliculas.Models.DTOs
{
    public class PeliculaCreateDTO
    {
        

        // Esto lo que hace es que sea obligatorio el nombre, y que salga un error si lo dejan en blanco
        // le agregamos un mensaje personalizado

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }



       
        public string RutaImagen { get; set; }

        // IFormFile , indicamos que es un campo de subida de archivo, por aqui permitimos la subida de imagen a nuestra api
      
        public IFormFile Foto { get; set; }

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
