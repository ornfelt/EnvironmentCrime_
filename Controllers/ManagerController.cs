using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EnvironmentCrime.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EnvironmentCrime.Controllers
{
  //only the roles manager can access manager views
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {

        private IEnvironmentRepository repository;
        private IHttpContextAccessor contextAcc;

        public ManagerController(IEnvironmentRepository repo, IHttpContextAccessor cont)
        {
            repository = repo;
            contextAcc = cont;
        }
        // GET: /<controller>/
        public ViewResult CrimeManager(int id)
        {
            ViewBag.ID = id;
            var userName = contextAcc.HttpContext.User.Identity.Name;
            var mgrInfo = repository.Employees.Where(em => em.EmployeeId == userName).FirstOrDefault().DepartmentId; //gets managers department id
            ViewBag.ListOfEmployees = repository.Employees.Where(e => e.DepartmentId == mgrInfo);

            return View();
        }

        public ViewResult StartManager()
        {
            var userName = contextAcc.HttpContext.User.Identity.Name;
            var mgrInfo = repository.Employees.Where(em => em.EmployeeId == userName).FirstOrDefault().DepartmentId; //gets managers department id
            //string mgrDepartment = mgrInfo.DepartmentId;

            ViewBag.ListOfStatuses = repository.ErrandStatuses;
            ViewBag.ListOfEmployees = repository.Employees.Where(e => e.DepartmentId == mgrInfo);

            var errandList =
                from err in repository.Errands.Where(ed => ed.DepartmentId.Equals(mgrInfo))
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

        [HttpPost]
        public IActionResult SaveInvestigator(int id, string investigator, string reason, bool noAction)
        {
            repository.UpdateInvestigator(id, investigator, reason, noAction);
            return RedirectToAction("StartManager");
        }

        [HttpPost]
        public IActionResult SortManager(string submit, string status, string investigator, string casenumber)
        {
            var userName = contextAcc.HttpContext.User.Identity.Name;
            var mgrInfo = repository.Employees.Where(em => em.EmployeeId == userName).FirstOrDefault().DepartmentId; //gets managers department id

            ViewBag.ListOfStatuses = repository.ErrandStatuses;
            ViewBag.ListOfEmployees = repository.Employees.Where(e => e.DepartmentId == mgrInfo);

            if (submit == "Sök" && casenumber != null && casenumber != "")
            {
                var errandList =
                from err in repository.Errands.Where(er => er.RefNumber == casenumber)
                join stat in repository.ErrandStatuses on err.StatusId equals stat.StatusId
                join dep in repository.Departments.Where(de => de.DepartmentId.Equals(mgrInfo)) on err.DepartmentId equals dep.DepartmentId
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

                return View("StartManager");
            }else if(submit == "Hämta lista" && investigator != null && investigator != "Välj alla" && status != null && status != "Välj alla")
            {
                var errandList =
                from err in repository.Errands.Where(st => st.StatusId == status).Where(em => em.EmployeeId == investigator)
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
                return View("StartManager");

            }else if(submit == "Hämta lista" && investigator != null && investigator != "Välj alla")
            {

                var errandList =
                from err in repository.Errands.Where(ed => ed.DepartmentId.Equals(mgrInfo)).Where(ed => ed.EmployeeId == investigator)
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
                return View("StartManager");

            }
            else if(submit == "Hämta lista" && status != null && status != "Välj alla")
            {
                var errandList =
                from err in repository.Errands.Where(ed => ed.DepartmentId.Equals(mgrInfo)).Where(er => er.StatusId == status)
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
                return View("StartManager");
            }
            else
            {
                return RedirectToAction("StartManager");
            }
        }
    }
}
