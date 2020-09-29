﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppBooks.Contexts;
using WebAppBooks.Models;
using WebAppBooks.Models.Login;

namespace WebAppBooks.Controllers.Login
{
    [Route("Login/[Controller]")]
    [ApiController]
    public class RolController:ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;


        public RolController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        [HttpPost("CrearRol")]
        public async Task<ActionResult<UserToken>> CrearRol([FromBody] RolDTO model)
        {            
            var user = new AspNetRoleManager { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                return BuildToken(model, new List<string>());
            }
            else
            {
                return BadRequest("Nombre de usuario o contraseña no válidos.");
            }

        }

        //    [HttpPost("Sign")]
        //    public async Task<ActionResult<UserToken>> Login([FromBody] UserInfo userInfo)
        //    {
        //        var result = await _signInManager.PasswordSignInAsync(userInfo.Email, userInfo.Password, isPersistent: false, lockoutOnFailure: false);
        //        if (result.Succeeded)
        //        {
        //            var usuario = await _userManager.FindByEmailAsync(userInfo.Email);
        //            var roles = await _userManager.GetRolesAsync(usuario);
        //            return BuildToken(userInfo, roles);
        //        }
        //        else
        //        {
        //            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        //            return BadRequest(ModelState);
        //        }
        //    }

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
