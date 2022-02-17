using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EnvironmentCrime.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EnvironmentCrime.Controllers
{
  //only the role Investigator can access investigator views
    [Authorize(Roles = "Investigator")]
    public class InvestigatorController : Controller
    {

        private IEnvironmentRepository repository;
        private IHostingEnvironment environment;
        private IHttpContextAccessor contextAcc;

        public InvestigatorController(IEnvironmentRepository repo, IHostingEnvironment env, IHttpContextAccessor cont)
        {
            repository = repo;
            environment = env;
            contextAcc = cont;
        }

        // GET: /<controller>/
        public ViewResult CrimeInvestigator(int id)
        {
            ViewBag.ID = id;
            ViewBag.ListOfStatuses = repository.ErrandStatuses;
            return View();
        }

        public ViewResult StartInvestigator()
        {
            var userName = contextAcc.HttpContext.User.Identity.Name;
            ViewBag.ListOfStatuses = repository.ErrandStatuses;

            var errandList =
                from err in repository.Errands.Where(ed => ed.EmployeeId.Equals(userName))
                join stat in repository.ErrandStatuses on err.StatusId equals stat.StatusId
                join dep in repository.Departments on err.DepartmentId equals dep.DepartmentId
                    into departmentErrand
                from deptE in departmentErrand.DefaultIfEmpty()
                join em in repository.Employees on err.EmployeeId equals em.EmployeeId
                    into employeeErrand
                from empE in employeeErrand.DefaultIfEmpty()
                orderby err.RefNumber descending
                select new ErrandConnect
                {
                    DateOfObservation = err.DateOfObservation,
                    ErrandId = err.ErrandID,
                    RefNumber = err.RefNumber,
                    TypeOfCrime = err.TypeOfCrime,
                    StatusName = stat.StatusName,
                    DepartmentName =
                        (err.DepartmentId == null ? "ej tillsatt" : deptE.DepartmentName),
                    EmployeeName =
                        (err.EmployeeId == null ? "ej tillsatt" : empE.EmployeeName)
                };
            ViewBag.ListOfErrands = errandList;

            return View(repository);
        }

        [HttpPost]
        public async Task<IActionResult> SaveInfoEntered(int id, string events, string information, string status, IFormFile loadSample, IFormFile loadImage)
        {
            var errand = repository.GetErrandInfo(id);
            if (status != "" && status != "Välj")
            {
                repository.UpdateErrandStatus(id, status);
            }

            if (events != "")
            {
                repository.UpdateInvestigatorAction(id, events);
            }

            if (information != "")
            {
                repository.UpdateInvestigatorInfo(id, information);
            }

            if (loadSample != null && loadSample.Length > 0)
            {
                //temporary search path
                var tempPath = Path.GetTempFileName();

                using (var stream = new FileStream(tempPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    await loadSample.CopyToAsync(stream);
                }

                //new search path
                var samplePath = Path.Combine(environment.WebRootPath, "uploads", loadSample.FileName);
                //var realFileType = Path.GetExtension(samplePath);

                //move temp file to path
                try
                {
                    System.IO.File.Move(tempPath, samplePath);
                }
                catch (Exception e)
                {
                    Console.WriteLine("The process failed: {0}", e.ToString());
                }
                repository.UpdateSample(id, loadSample.FileName);
            }

            if (loadImage != null && loadImage.Length > 0)
            {
                //temporary search path
                var tempPath2 = Path.GetTempFileName();

                using (var stream = new FileStream(tempPath2, FileMode.Create, FileAccess.ReadWrite))
                {
                    await loadImage.CopyToAsync(stream);
                }

                //new search path
                var imagePath = Path.Combine(environment.WebRootPath, "uploads", loadImage.FileName);
                //var realFileTypeImage = Path.GetExtension(imagePath);

                //move temp file to path
                try
                {
                    System.IO.File.Move(tempPath2, imagePath);
                }
                catch (Exception e)
                {
                    Console.WriteLine("The process failed: {0}", e.ToString());
                }
                repository.UpdatePicture(id, loadImage.FileName);
            }

            return RedirectToAction("StartInvestigator");
        }

        [HttpPost]
        public IActionResult SortInvestigator(string submit, string status, string casenumber)
        {
            var userName = contextAcc.HttpContext.User.Identity.Name;
            ViewBag.ListOfStatuses = repository.ErrandStatuses;

            if (submit == "Sök" && casenumber != null && casenumber != "")
            {
                var errandList =
                from err in repository.Errands.Where(er => er.RefNumber == casenumber).Where(ed => ed.EmployeeId.Equals(userName))
                join stat in repository.ErrandStatuses on err.StatusId equals stat.StatusId
                join dep in repository.Departments on err.DepartmentId equals dep.DepartmentId
                    into departmentErrand
                from deptE in departmentErrand.DefaultIfEmpty()
                join em in repository.Employees on err.EmployeeId equals em.EmployeeId
                    into employeeErrand
                from empE in employeeErrand.DefaultIfEmpty()
                orderby err.RefNumber descending
                select new ErrandConnect
                {
                    DateOfObservation = err.DateOfObservation,
                    ErrandId = err.ErrandID,
                    RefNumber = err.RefNumber,
                    TypeOfCrime = err.TypeOfCrime,
                    StatusName = stat.StatusName,
                    DepartmentName =
                        (err.DepartmentId == null ? "ej tillsatt" : deptE.DepartmentName),
                    EmployeeName =
                        (err.EmployeeId == null ? "ej tillsatt" : empE.EmployeeName)
                };

                ViewBag.ListOfErrands = errandList;

                return View("StartInvestigator");
            }
            else if (submit == "Hämta lista" && status != null && status != "Välj alla")
            {
                //var inputStatusId = repository.ErrandStatuses.Where(es => es.StatusName == status).First().StatusName;

                var errandList =
                from err in repository.Errands.Where(ed => ed.EmployeeId.Equals(userName)).Where(er => er.StatusId == status)
                join stat in repository.ErrandStatuses on err.StatusId equals stat.StatusId
                join dep in repository.Departments on err.DepartmentId equals dep.DepartmentId
                    into departmentErrand
                from deptE in departmentErrand.DefaultIfEmpty()
                join em in repository.Employees on err.EmployeeId equals em.EmployeeId
                    into employeeErrand
                from empE in employeeErrand.DefaultIfEmpty()
                orderby err.RefNumber descending
                select new ErrandConnect
                {
                    DateOfObservation = err.DateOfObservation,
                    ErrandId = err.ErrandID,
                    RefNumber = err.RefNumber,
                    TypeOfCrime = err.TypeOfCrime,
                    StatusName = stat.StatusName,
                    DepartmentName =
                        (err.DepartmentId == null ? "ej tillsatt" : deptE.DepartmentName),
                    EmployeeName =
                        (err.EmployeeId == null ? "ej tillsatt" : empE.EmployeeName)
                };

                ViewBag.ListOfErrands = errandList;
                return View("StartInvestigator");
            }
            else
            {
                return RedirectToAction("StartInvestigator");
            }
        }
        }
}
