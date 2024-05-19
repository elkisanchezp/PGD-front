using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TesisMarco.Models;
using System.Security.Claims;
using TesisMarco.DTO;
using RestSharp;
using Newtonsoft.Json;

namespace TesisMarco.Controllers
{
    public class LoginController : Controller
    {
        // GET: LoginController
        public IActionResult Index(string returnUrl = "/")
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Index(LoginModel model, string returnUrl = "/")
        {

            // Verificar si el correo electrónico y la contraseña son correctos
            if (!string.IsNullOrEmpty(model.Usuario) && !string.IsNullOrEmpty(model.Password))
            {
                string apiUrlLogin = $"https://pgd-app.onrender.com/api/usuario/login";

                // Crear cliente RestSharp
                var client = new RestClient(apiUrlLogin);

                // Crear solicitud GET
                var request = new RestRequest("", Method.Post);

                request.AddJsonBody(new { username = model.Usuario, password = model.Password });

                // Ejecutar la solicitud y obtener la respuesta
                var response = client.Execute(request);

                if (response.IsSuccessful && response.Content != null)
                {
                    var usuarioApi = JsonConvert.DeserializeObject<Usuario>(response.Content);

                    // Asignar roles al usuario
                    var roles = new List<string> { usuarioApi.tipoUsuario }; // Roles a asignar
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Usuario),
                        // Puedes añadir más claims según necesites
                    };

                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    claims.Add(new Claim("idEntidad", usuarioApi.codigoentidad.ToString()));


                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Content);
                }
            }
            else
            {
                // Credenciales incorrectas, mostrar mensaje de error
                ModelState.AddModelError(string.Empty, "Correo electrónico o contraseña incorrectos.");
               
            }
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }


        // GET: LoginController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: LoginController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LoginController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LoginController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LoginController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LoginController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LoginController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
