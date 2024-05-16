using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using RestSharp;
using TesisMarco.DTO;
using TesisMarco.Models;

namespace TesisMarco.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        // GET: UsuariosController
        public ActionResult Index()
        {

            string apiUrlEntidades = "https://pgd-app.onrender.com/api/entidades";

            // Crear cliente RestSharp
            var client = new RestClient(apiUrlEntidades);

            // Crear solicitud GET
            var request = new RestRequest("", Method.Get);

            // Ejecutar la solicitud y obtener la respuesta
            var response = client.Execute(request);

            List<Entidad> entidades = new List<Entidad>();

            if (response.IsSuccessful)
            {
                // Deserializar la respuesta JSON en una lista de objetos
                entidades = JsonConvert.DeserializeObject<List<Entidad>>(response.Content);
            }

            ViewBag.Entidades = new SelectList(entidades, "codigoSigep", "nombre");

            var viewModel = new UsuarioModel(); 
            return View(viewModel);
        }


        [HttpPost]
        public IActionResult CrearUsuario(UsuarioModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string apiUrl = "https://pgd-app.onrender.com/api/usuario";

                    // Crear cliente RestSharp
                    var client = new RestClient(apiUrl);

                    // Crear solicitud POST
                    var request = new RestRequest("", Method.Post);
                    // ADMIN - NORMAL

                    request.AddJsonBody(new { username = model.Usuario, password = model.Password, tipoUsuario = (model.IdTipoUsuario == 1 ?"ADMIN":"NORMAL"), codigoentidad= model.IdEntidad });

                    // Ejecutar la solicitud y obtener la respuesta
                    var response = client.Execute(request);



                    return RedirectToAction("Index", "Home"); // Redirigir a la página de inicio u otra página
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Ocurrió un error al guardar el usuario. Por favor, inténtalo de nuevo.");
                    // Log the exception
                }
            }

            // Si llegamos aquí, significa que hubo un error, volvemos a mostrar el formulario con los errores
            return RedirectToAction("Index", "Usuarios"); ;
        }


        // GET: UsuariosController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UsuariosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsuariosController/Create
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

        // GET: UsuariosController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UsuariosController/Edit/5
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

        // GET: UsuariosController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UsuariosController/Delete/5
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
