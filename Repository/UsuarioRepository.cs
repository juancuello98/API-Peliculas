using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiPeliculas.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        // Instanciamos la cadena de conexion

        private readonly ApplicationDbContext _dataBase;

        public UsuarioRepository(ApplicationDbContext dataBase)
        {
            _dataBase = dataBase;
        }

        public bool ExisteUsuario(string nombreUsuario)
        {
            if(_dataBase.Usuario.Any(x => x.UsuarioA == nombreUsuario))
            {
                return true;
            }
            return false;
        }

        public Usuario GetUsuario(int usuarioId)
        {
            return _dataBase.Usuario.FirstOrDefault(c => c.Id == usuarioId);
        }

        public ICollection<Usuario> GetUsuarios()
        {
            return _dataBase.Usuario.OrderBy(c => c.UsuarioA).ToList();
        }

        public bool Guardar()
        {
            return _dataBase.SaveChanges() >= 0 ? true : false;
        }

        public Usuario Login(string nombreUsuario, string password)
        {
            var user = _dataBase.Usuario.FirstOrDefault(x => x.UsuarioA == nombreUsuario);
            if (user == null)
            {
                return null;
            }

            // Validaciones para el password antes del Hash

            if (!VerificaPasswordHash(password, user.PasswordHash,user.PasswordSalt))
            {
                // Entra si no valida el password hash con esos parametros

                return null;
            }
            return user;
        }

        
        public Usuario Registro(Usuario usuario, string password)
        {
            byte[] passwordHash, passwordSalt;

            CrearPasswordHash(password, out passwordHash,out passwordSalt);

            usuario.PasswordHash = passwordHash;
            usuario.PasswordSalt = passwordSalt;

            _dataBase.Usuario.Add(usuario);

            Guardar();
            return usuario;
        }

        private void CrearPasswordHash(string password, out byte[] passwordHash,out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

        }

        private bool VerificaPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt) )
            {
                var hashComputado = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < hashComputado.Length; i++)
                {
                    if (hashComputado[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }

}
