using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EnvironmentCrime.Models;

namespace EnvironmentCrime.Components
{
    public class CrimeErrandViewComponent:ViewComponent //inherits from base class
    {
        private IEnvironmentRepository repository;

        public CrimeErrandViewComponent(IEnvironmentRepository repo)
        {
            repository = repo;
        }
    /*
     * Task that is called from cshtml that shows deatails for the errand clicked on
     */
        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
      string departmentName = null;
      string employeeName = null;
            var objectOfErrand = await repository.GetErrandDetail(id);
            var statusName = repository.ErrandStatuses.Where(es => es.StatusId == objectOfErrand.StatusId).FirstOrDefault().StatusName; //gets statusname for errand
      if (objectOfErrand.DepartmentId != null)
      {
        var depName = repository.Departments.Where(de => de.DepartmentId == objectOfErrand.DepartmentId).FirstOrDefault().DepartmentName; //gets departmentname for errand
        departmentName = depName;
      }
      if(objectOfErrand.EmployeeId != null && objectOfErrand.EmployeeId != "ej tillsatt")
      {
        //gets employeename for errand
        var empName = repository.Employees.Where(em => em.EmployeeId == objectOfErrand.EmployeeId).FirstOrDefault().EmployeeName;
        employeeName = empName;
      }

            ViewBag.ErrandConnecter = new ErrandConnect
            {
                DateOfObservation = objectOfErrand.DateOfObservation,
                ErrandId = objectOfErrand.ErrandID,
                RefNumber = objectOfErrand.RefNumber,
                TypeOfCrime = objectOfErrand.TypeOfCrime,
                StatusName = statusName,
                DepartmentName =
                        (objectOfErrand.DepartmentId == null ? "ej tillsatt" : departmentName),
                EmployeeName =
                        (objectOfErrand.EmployeeId == null ? "ej tillsatt" : employeeName)
            };

            if (repository.SamplePath(id) == ""){
                ViewBag.SampleName = "";
            }
            else
            {
                ViewBag.SampleName = repository.SamplePath(id);
            }

            if (repository.PicturePath(id) == "")
            {
                ViewBag.PictureName = "";
            }
            else
            {
                ViewBag.PictureName = repository.PicturePath(id);
            }

            return View(objectOfErrand);
        }
    }
}
