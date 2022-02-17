using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EnvironmentCrime.Models;

namespace EnvironmentCrime.Components
{
    public class PictureColumnViewComponent : ViewComponent
    {
        private IEnvironmentRepository repository;

        public PictureColumnViewComponent(IEnvironmentRepository repo)
        {
            repository = repo;
        }

        /*
        * VC that is called from cshtml showing the aside column with pictures 
        */
        public IViewComponentResult Invoke()
        {
            return View("PictureColumn");
        }
    }
}
