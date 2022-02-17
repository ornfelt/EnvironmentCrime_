using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EnvironmentCrime.Models;
using EnvironmentCrime.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EnvironmentCrime.Controllers
{
    [Authorize(Roles = "Coordinator")] //only the coordinator role can access the coordinator views
    public class CoordinatorController : Controller
    {

        private IEnvironmentRepository repository;
        private IHttpContextAccessor contextAcc;

        public CoordinatorController(IEnvironmentRepository repo, IHttpContextAccessor cont)
        {
            repository = repo;
            contextAcc = cont;
        }

    //shows view with all the crime selected
        public ViewResult CrimeCoordinator(int id)
        {
            ViewBag.ID = id;
            return View(repository.Departments);
        }

    //shows view where the user can report the new crime
        public ViewResult ReportCrime()
        {
            var myErrand = HttpContext.Session.GetJson<Errand>("NewErrand");

            if (myErrand == null)
            {
                return View();
            }
            else
            {
                return View(myErrand);
            }
        }

    //shows view with all errands
        public ViewResult StartCoordinator()
        {
            var userName = contextAcc.HttpContext.User.Identity.Name;
            ViewBag.ListOfStatuses = repository.ErrandStatuses;
            ViewBag.ListOfDepartments = repository.Departments;

            var errandList =
                from err in repository.Errands
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

            return View();
        }

    //shows view thanks, that shows the errand reported
        public ViewResult Thanks(Errand errand)
        {
            errand = HttpContext.Session.GetJson<Errand>("NewErrand");
            ViewBag.refNo = repository.SaveErrand(errand);
            HttpContext.Session.Remove("NewErrand");
            return View(errand);
    }

    //shows view validate and creates new session if modelstate is correct
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

        public IActionResult SaveDepartment(int id, string department)
        {
            repository.UpdateErrandDepartment(id, department);
            return RedirectToAction("StartCoordinator");
        }

        [HttpPost]
        public IActionResult SortCoordinator(string submit, string status, string department, string casenumber)
        {
            ViewBag.ListOfStatuses = repository.ErrandStatuses;
            ViewBag.ListOfDepartments = repository.Departments;

            if (submit == "Sök" && casenumber != null && casenumber != "")
            {
                var errandList =
                from err in repository.Errands.Where(er => er.RefNumber == casenumber)
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

                return View("StartCoordinator");
            }
            else if (submit == "Hämta lista" && department != null && department != "Välj alla" && status != null && status != "Välj alla")
            {
                var errandList =
                from err in repository.Errands.Where(ed => ed.DepartmentId.Equals(department)).Where(er => er.StatusId == status)
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
                return View("StartCoordinator");

            }
            else if (submit == "Hämta lista" && department != null && department != "Välj alla")
            {
                var errandList =
                from err in repository.Errands.Where(ed => ed.DepartmentId.Equals(department))
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
                return View("StartCoordinator");

            }
            else if (submit == "Hämta lista" && status != null && status != "Välj alla")
            {
                //var inputStatusId = repository.ErrandStatuses.Where(es => es.StatusName == status).First().StatusName;

                var errandList =
                from err in repository.Errands.Where(er => er.StatusId == status)
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
                return View("StartCoordinator");
            }
            else
            {
                return RedirectToAction("StartCoordinator");
            }
        }
        }
}
