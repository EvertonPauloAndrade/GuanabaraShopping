using Microsoft.AspNetCore.Mvc;
using ClienteManager.Data;
using ClienteManager.Models;
using ClienteManager.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace ClienteManager.Controllers
{
    public class AccountController : Controller
    {
        private readonly JsonDataService _dataService;

        public AccountController(JsonDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var users = _dataService.GetUsers();
                if (users.Any(u => u.UserName.Equals(model.UserName, StringComparison.OrdinalIgnoreCase)))
                {
                    ModelState.AddModelError("UserName", "Este nome de usuário já existe.");
                    return View(model);
                }

                var newUser = new ApplicationUser
                {
                    UserName = model.UserName,
                    PasswordHash = _dataService.HashPassword(model.Password)
                };

                users.Add(newUser);
                _dataService.SaveUsers(users);

                await SignInUser(newUser);

                return RedirectToAction("Index", "Cliente");
            }

            Console.WriteLine("Usuário tentou logar com: " + model.UserName);
            Console.WriteLine("Senha recebida: " + model.Password);
            Console.WriteLine("Hash esperada: " + _dataService.HashPassword(model.Password));


            return View(model);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _dataService.GetUsers()
                                       .FirstOrDefault(u => u.UserName.Equals(model.UserName, StringComparison.OrdinalIgnoreCase));
                
                if (user != null && user.PasswordHash == _dataService.HashPassword(model.Password))
                {
                    await SignInUser(user);
                    return RedirectToAction("Index", "Cliente");
                }

                ModelState.AddModelError(string.Empty, "Tentativa de login inválida.");
            }

            Console.WriteLine("Usuário tentou logar com: " + model.UserName);
            Console.WriteLine("Senha recebida: " + model.Password);
            Console.WriteLine("Hash esperada: " + _dataService.HashPassword(model.Password));


            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Cliente");
        }
        
        private async Task SignInUser(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties { IsPersistent = true });
        }
    }
}