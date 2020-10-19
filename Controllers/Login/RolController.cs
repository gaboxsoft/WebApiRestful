using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAppBooks.Contexts;
using WebAppBooks.Models;
using WebAppBooks.Models.Login;

namespace WebAppBooks.Controllers.Login
{
    /// <summary>
    /// EndPoints para Crear, modificar, borrar o asignar un ROL de usuario
    /// </summary>
    [Route("users/[Controller]")]
    [ApiController]
    public class RolController:ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> rolManager;
        private readonly IMapper mapper;


        public RolController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> rolManager)
        {
            this.context = context;
            this.userManager = userManager;
            this.rolManager = rolManager;
        }

        /// <summary>
        /// Crea un Rol desde Body enviado en un objeto RolDTO
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        /// Si fue creado regresa un objeto RolDTO
        /// Si no fue creado regresa BadRequest()
        /// Si fue creado </returns>
        [HttpPost("Crear")]
        public async Task<ActionResult<RolDTO>> Crear([FromBody] RolDTO model)
        {      
            var rol = new IdentityRole { ConcurrencyStamp = Guid.NewGuid().ToString(), Name=model.Name, NormalizedName=model.Name};
            var result = await rolManager.CreateAsync(rol);
            if (result.Succeeded)
            {

                return new RolDTO { Name = rol.Name };
            }
            else
            {
                return BadRequest("Nombre de usuario o contraseña no válidos.");
            }

        }

        /// <summary>
        /// Adiciona un Rol a un Usuario. El Rol se especifíca desde el Body en u objeto EditarRolDTO.
        /// </summary>
        /// <param name="editarRolDTO"></param>
        /// <returns>
        /// Si no existe el usuario regresa NotFound();
        /// Si se logro asignar el rol al Usuario regresa Ok();
        /// </returns>
        [HttpPost("AddRolToUser")]
        public async Task<ActionResult<UserToken>> AgregarRolAUsuario([FromBody] EditarRolDTO editarRolDTO)
        {
            var usuario = await userManager.FindByIdAsync(editarRolDTO.UserId);
            if (usuario == null)
            {
                return NotFound();
            }

            await userManager.AddClaimAsync(usuario, new Claim(ClaimTypes.Role, editarRolDTO.RolName));
            await userManager.AddToRoleAsync(usuario, editarRolDTO.RolName);
            return Ok();
        }

        /// <summary>
        /// Quita el Rol a un Usuario. El Rol se especifíca desde el Body en u objeto EditarRolDTO.
        /// </summary>
        /// <param name="editarRolDTO"></param>
        /// <returns>
        /// Si no existe el usuario regresa NotFound();
        /// Si se logro asignar el rol al Usuario regresa Ok();
        ///</returns>
        [HttpPost("RemoveRolFromUser")]
        public async Task<ActionResult<UserToken>> QuitarRolDelUsuario([FromBody] EditarRolDTO editarRolDTO)
        {
            var usuario = await userManager.FindByIdAsync(editarRolDTO.UserId);
            if (usuario == null)
            {
                return NotFound();
            }

            await userManager.RemoveClaimAsync(usuario, new Claim(ClaimTypes.Role, editarRolDTO.RolName));
            await userManager.RemoveFromRoleAsync(usuario, editarRolDTO.RolName);
            return Ok();
        }

        /// <summary>
        /// Obtiene una lista con todos los Roles existentes en una lista de tipo RolDTO.
        /// </summary>        
        /// <returns>
        /// Si no existen roles regresa una lista vacía.
        /// Si existen Roles regresa una lista de tipo RolDTO
        ///</returns>
        [HttpGet("Get")]
        public async Task<ActionResult<List<RolDTO>>> Get()
        {
            //List<RolDTO> roles = mapper.Map<List<RolDTO>>((await context.Roles.ToListAsync()));
            var roles = await context.Roles.ToListAsync();

            if (roles == null)
            {
                roles = new List<IdentityRole>();
                //return NotFound();
            }
            List<RolDTO> rolesDTO = new List<RolDTO>();
            foreach (var rol in roles)
            {
                rolesDTO.Add(new RolDTO {Name=rol.Name });
            }
            //return mapper.Map<List<RolDTO>>(roles);
            return rolesDTO;
        }


        //    private UserToken BuildToken(UserInfo userInfo, IList<string> roles)
        //    {
        //        var claims = new List<Claim>
        //        {
        //    new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.Email),
        //    new Claim("miValor", "Lo que yo quiera"),
        //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        //};

        //        foreach (var rol in roles)
        //        {
        //            claims.Add(new Claim(ClaimTypes.Role, rol));
        //        }

        //        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]));
        //        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //        // Tiempo de expiración del token. En nuestro caso lo hacemos de una hora.
        //        //var expiration = DateTime.UtcNow.AddYears(1);
        //        var expiration = DateTime.Now.AddMinutes(10);

        //        JwtSecurityToken token = new JwtSecurityToken(
        //           issuer: null,
        //           audience: null,
        //           claims: claims,
        //           expires: expiration,
        //           signingCredentials: creds);

        //        return new UserToken()
        //        {
        //            Token = new JwtSecurityTokenHandler().WriteToken(token),
        //            Expiration = expiration
        //        };
    }
}
