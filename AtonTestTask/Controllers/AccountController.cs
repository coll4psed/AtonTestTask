using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AtonTestTask.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace AtonTestTask.Controllers
{
    public class AccountController : Controller
    {
        private AtonTestTask.Context.AtonTestTaskContext db;
        public AccountController(AtonTestTask.Context.AtonTestTaskContext context)
        {
            db = context;
        }
/*
        [HttpPost("/login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Login == username && u.Password == password);
            if (user != null)
            {
                await Authenticate(user);

                return Ok($"Пользователь {HttpContext.User.Identity.Name} авторизован");
            }

            return StatusCode(500, "Что-то пошло не так");
        }*/

        private async Task Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Admin.ToString()),
                new Claim("CreatedOn", user.CreatedOn.ToShortDateString()),
                new Claim("CreatedBy", user.CreatedBy),
                new Claim("ModifiedOn", user.ModifiedOn.ToString() ?? "0000-00-00"),
                new Claim("ModifiedBy", user.ModifiedBy ?? "Не изменялся"),
                new Claim("RevokedOn", user.RevokedOn.ToString() ?? "0000-00-00"),
                new Claim("RevokedBy", user.RevokedBy ?? "Не удалялся")
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}
