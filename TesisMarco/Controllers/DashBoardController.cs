using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TesisMarco.Controllers
{
    [Authorize]
    public class DashBoardController : Controller
    {
        // GET: DashBoardController
        public ActionResult Index()
        {
            return View();
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
