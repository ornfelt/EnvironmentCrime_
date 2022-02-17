using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EnvironmentCrime.Models;
using Microsoft.AspNetCore.Http;
using System.Net;
using EnvironmentCrime.Infrastructure;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EnvironmentCrime.Controllers
{
    public class CitizenController : Controller
    {
        private IEnvironmentRepository repository;

        public CitizenController(IEnvironmentRepository repo)
        {
            repository = repo;
        }

        // GET: /<controller>/
        public IActionResult Contact()
        {
            return View();
        }

        public ViewResult Faq()
        {
            return View();
        }

        public ViewResult Services()
        {
            return View();
        }

      //shows thanks view, the errand is sent with the view, and the session is removed
        public ViewResult Thanks(Errand errand)
        {
            errand = HttpContext.Session.GetJson<Errand>("NewErrand");
            ViewBag.refNo = repository.SaveErrand(errand);
            HttpContext.Session.Remove("NewErrand");
            return View(errand);
        }

    //shows validate view, a new session is created if the modelstate is valid
        public ViewResult Validate(Errand errand)
        {
            if (ModelState.IsValid)
            {
                HttpContext.Session.SetJson("NewErrand", errand);
                return View(errand);
            }
            else
            {
                return View();
            }
        }
    }
}
