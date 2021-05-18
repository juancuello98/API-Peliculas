using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Models
{
    public class Pelicula
    {
        [Key]
        public int Id { get; set; }

        // Esto lo que hace es que sea obligatorio el nombre, y que salga un error si lo dejan en blanco
        // le agregamos un mensaje personalizado

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        public string RutaImagen { get; set; }

        public string Descripcion { get; set; }

        public string Duracion { get; set; }
        public enum TipoDeClasificacion { Siete, Trece, Dieciseis, Dieciocho }

        public TipoDeClasificacion Clasificacion { get; set; }
        public DateTime FechaCreacion { get; set; }



        //Estos campos me relacionan la tabla categorias con peliculas
        public int categoriaId { get; set; }

        [ForeignKey("categoriaId")]
        public Categoria Categoria { get; set; }
    }
}
