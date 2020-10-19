using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAppBooks.Models;
using WebAppBooks.Models.Login;

namespace WebAppBooks.Controllers.Login
{

    /// <summary>
    /// Administra los usuarios y sus logins 
    /// </summary>

    [Route("users/[controller]")]
    [ApiController]
    public class CuentaController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CuentaController> logger;

        public CuentaController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration, ILogger<CuentaController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            this.logger = logger;
        }

        /// <summary>
        /// Crea un Usuario con datos desde el Body en u objeto tipo UserInfo
        /// </summary>
        /// <param name="model"></param>
        /// <returns>
        /// Regresa un objeto UserToken si fue creado
        /// BadRequest() si hubo error: "Nombre de usuario o contraseña no válidos."</returns>
        [HttpPost("Crear")]
        public async Task<ActionResult<UserToken>> CreateUser([FromBody] UserInfo model)
        {
             var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                logger.LogWarning($" {DateTime.Now}: USUARIO CREADO ({model.Email}, passwd: {model.Password})");
                return BuildToken(model, new List<string>());
            }
            else
            {
                logger.LogInformation($" {DateTime.Now}: INTENTO CREAR USUARIO ({model.Email}, passwd: {model.Password})");
                return BadRequest("Nombre de usuario o contraseña no válidos.");
            }

        }

        /// <summary>
        ///  Realiza un intento de Login usando datos desde el Body en un objeto UserInfo. Regresa Token en un objeto UserToken.
        /// </summary>
        /// <param name="userInfo"> FromBody de tipo UserInfo</param>
        /// <returns>
        /// Un objeto UserToken si se lorgó registrar
        /// Un BadRequest() si hubo un error: "Intento de login inválido." </returns>
        [HttpPost("Sign")]
        public async Task<ActionResult<UserToken>> Login([FromBody] UserInfo userInfo)
        {
            var result = await _signInManager.PasswordSignInAsync(userInfo.Email, userInfo.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                logger.LogInformation($" {DateTime.Now}: INICIO SESION OK ({userInfo.Email}, passwd: {userInfo.Password})");
                var usuario = await _userManager.FindByEmailAsync(userInfo.Email);
                var roles = await _userManager.GetRolesAsync(usuario);
                return BuildToken(userInfo, roles);
            }
            else
            {
                logger.LogWarning($" {DateTime.Now}: INICIO SESION FALLIDA ({userInfo.Email}, passwd: {userInfo.Password})");
                ModelState.AddModelError(string.Empty, "Intento de login inválido.");
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// Regresa un objeto UserToken usando datos del usuario y sus roles.
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        private UserToken BuildToken(UserInfo userInfo, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.Email),
                new Claim("miValor", "Lo que yo quiera"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var rol in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol));
            }

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Tiempo de expiración del token. En nuestro caso lo hacemos de una hora.
            //var expiration = DateTime.UtcNow.AddYears(1);
            var expiration = DateTime.Now.AddMinutes(10);

            JwtSecurityToken token = new JwtSecurityToken(
               issuer: null,
               audience: null,
               claims: claims,
               expires: expiration,
               signingCredentials: creds);

            return new UserToken()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}
