using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EnvironmentCrime.Models;

namespace EnvironmentCrime.Components {
  public class DropDownViewComponent : ViewComponent {
    private IEnvironmentRepository repository;

    public DropDownViewComponent(IEnvironmentRepository repo)
    {
      repository = repo;
    }

    /*
    * VC that is called from cshtml - the VC shows the dropdown list for errandstatuses
    */
    public IViewComponentResult Invoke()
    {
      ViewBag.ListOfStatuses = repository.ErrandStatuses;
      return View("DropDown");
    }
  }
}
