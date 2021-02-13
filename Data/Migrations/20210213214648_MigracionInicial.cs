using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ApiPeliculas.Migrations
{
    public partial class MigracionInicial : Migration
    {
        // todo esto se creo con el comando add-migration MigracionInicial
        
        /* El metodo Up lo que me dice que es lo que se va a crear
            - lo que me dice este metodo es que se va a crear una tabla categoria
        que va a tener un id que va a ser entero y identity:true , con los campos que tengo 
        en el modelo.

        Si esto saliera todo vacio es porque en el ApplicationDbContext no tenemos el DbSet
         */
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCategoria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categoria", x => x.Id);
                });
        }

        /* El metodo Down lo que me dice que es lo que se va a borrar
         por si quisiera borrar eso
         */
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categoria");
        }
    }
}
