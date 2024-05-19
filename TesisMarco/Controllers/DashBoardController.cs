using Microsoft.AspNetCore.Authorization;
using TesisMarco.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TesisMarco.Controllers
{
    [Authorize]
    public class DashBoardController : Controller
    {
        // GET: DashBoardController
        public ActionResult Index()
        {
            int idEntidad = 0;

            var idEntidadClaim = User.FindFirst("idEntidad")?.Value;
            if (idEntidadClaim != null)
                idEntidad = int.Parse(idEntidadClaim);

            var entidades = ObtenerEntidades().OrderBy(x=> x.nombre);
            ViewBag.Entidades = new SelectList(entidades, "codigoSigep", "nombre");
            ViewBag.NombreEntidad = entidades.Where(x => x.codigoSigep == idEntidad).FirstOrDefault()?.nombre;
            var modelo = ObtenerEntidadPuntaje(idEntidad);

            return View(modelo);
        }


        private List<Entidad> ObtenerEntidades()
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

            return entidades ?? new List<Entidad>();
        }

        private EntidadPuntaje ObtenerEntidadPuntaje(int codigosidep)
        {
            string apiUrlEntidades = $"https://pgd-app.onrender.com/api/puntajes/entidad/{codigosidep}";

            // Crear cliente RestSharp
            var client = new RestClient(apiUrlEntidades);

            // Crear solicitud GET
            var request = new RestRequest("", Method.Get);

            // Ejecutar la solicitud y obtener la respuesta
            var response = client.Execute(request);

            EntidadPuntaje entidades = new EntidadPuntaje();

            if (response.IsSuccessful)
            {
                // Deserializar la respuesta JSON en una lista de objetos
                entidades = JsonConvert.DeserializeObject<EntidadPuntaje>(response.Content);
            }

            return entidades ?? new EntidadPuntaje();
        }


        public JsonResult ObtenerPuntaje(int idEntidad)
        {
            var modelo = ObtenerEntidadPuntaje(idEntidad);
            return Json(modelo);
        }



        // GET: DashBoardController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DashBoardController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DashBoardController/Create
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

        // GET: DashBoardController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DashBoardController/Edit/5
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

        // GET: DashBoardController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DashBoardController/Delete/5
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
