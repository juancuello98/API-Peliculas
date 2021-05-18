using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiPeliculas.Data;
using ApiPeliculas.Repository;
using ApiPeliculas.Repository.IRepository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ApiPeliculas.PeliculasMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Reflection;
using AutoMapper;
using System.IO;
using Microsoft.OpenApi.Models;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using ApiPeliculas.Helpers;

namespace ApiPeliculas
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Creamos la cadena de conexion
            // Debemos instalar para el UseSqlServer el package Microsoft.EntityFrameworkCore.SqlServer
            // y asi despues poder hacer uso del using EntityFrameworkCore

            services.AddDbContext<ApplicationDbContext>(Options => Options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Agregamos esto (inyeccion de dependencias?)
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<IPeliculaRepository, PeliculaRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            /*Agregaos dependencia del token*/
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    //Construimos lo que necesitamos para nuestro token 
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            //Token implementado hasta aca.

            //Agregamos el automapper, sin esto no funciona, para que se usa el typeof?

            services.AddAutoMapper(typeof(PeliculasMappers)); // Aca nos da un error sin antes poner en plural el mapper con nombre distinto al namespace
            // De aqui en adelante configuracion de documentacion de nuestra API

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("ApiPeliculasCategorias", new Microsoft.OpenApi.Models.OpenApiInfo()
                {

                    Title = "API Categorias",
                    Version = "1",
                    Description = "Backend peliculas",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                    {
                        Email = "juan.frc.utn@gmail.com",
                        Name = "playMovie",
                        Url = new Uri("https://playmovie.com.ar")

                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense()
                    {
                        Name = "MIT License",
                        Url = new Uri("https://en.wikipedia.org/wiki/MIT_License")
                    }
                });

                options.SwaggerDoc("ApiPeliculas", new Microsoft.OpenApi.Models.OpenApiInfo()
                {

                    Title = "API Peliculas",
                    Version = "1",
                    Description = "Backend peliculas",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                    {
                        Email = "juan.frc.utn@gmail.com",
                        Name = "playMovie",
                        Url = new Uri("https://playmovie.com.ar")

                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense()
                    {
                        Name = "MIT License",
                        Url = new Uri("https://en.wikipedia.org/wiki/MIT_License")
                    }
                });


                options.SwaggerDoc("ApiPeliculasUsuarios", new Microsoft.OpenApi.Models.OpenApiInfo()
                {

                    Title = "API Usuarios",
                    Version = "1",
                    Description = "Backend peliculas",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                    {
                        Email = "juan.frc.utn@gmail.com",
                        Name = "playMovie",
                        Url = new Uri("https://playmovie.com.ar")

                    },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense()
                    {
                        Name = "MIT License",
                        Url = new Uri("https://en.wikipedia.org/wiki/MIT_License")
                    }
                });

                // Ingresamos una variable con el archivo XML para los comentarios describiendo que hacen los metodos HTTP de la api

                var archivoXmlComentarios = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

                var rutaApiComentarios = Path.Combine(AppContext.BaseDirectory, archivoXmlComentarios);
                options.IncludeXmlComments(rutaApiComentarios);

                options.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description = "Autenticacion JWT (Bearer)",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer"
                    });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement{
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    }, new List<string>()

                } 
                 });
            });

            /*Damos soporte para CORS*/
            services.AddCors(); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) //valida si estamos trabajando en un entorno de desarrollo
            {
                app.UseDeveloperExceptionPage();
            }
            else // este else maneja los errores de forma global cuando estamos en production 
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>      // Empezamos a crear el bloque de codigo, accedemos al contexto, a la peticion y a la respuesta HTTP de manera global
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // lo convierte en entero
                        var error = context.Features.Get<IExceptionHandlerFeature>();

                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message); // Necesitamos un archivo capturador de errores por eso tnemos la carpeta Helpers
                        }
                    });
                });
            }
            app.UseHttpsRedirection();

            // Anadimos una linea para documentar la api
            app.UseSwagger();
            app.UseSwaggerUI(option => 
            {
                option.SwaggerEndpoint("/swagger/ApiPeliculasCategorias/swagger.json", "API Categorias");
                option.SwaggerEndpoint("/swagger/ApiPeliculas/swagger.json", "API Peliculas");
                option.SwaggerEndpoint("/swagger/ApiPeliculasUsuarios/swagger.json", "API Usuarios");
                option.RoutePrefix = "";
            });

            app.UseRouting();

            /* Estos son para la autenticacion y autorizacion*/
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            /*Dando soporte para cors -> hacer uso de la api desde un dominio diferente*/
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()); // Esto tiene que cambiarse y darle un acceso mas restringido limitandolo solo a los dominios de los clientes autorizados

            
        
        }
    }
}
