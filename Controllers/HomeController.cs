using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EnvironmentCrime.Models;
using EnvironmentCrime.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EnvironmentCrime.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private UserManager<IdentityUser> userManager;
        private SignInManager<IdentityUser> signInManager;
        private IEnvironmentRepository repository;

        public HomeController(IEnvironmentRepository repo, UserManager<IdentityUser> userMgr, SignInManager<IdentityUser> signInMgr)
        {
            repository = repo;
            userManager = userMgr;
            signInManager = signInMgr;
        }

    //shows view index 
        public ViewResult Index()
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
        public ViewResult ReceiveForm()
        {
            return View();
        }

    //shows login view 
        [AllowAnonymous]
        public ViewResult Login(string returnUrl)
        {
            return View(new LoginModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken] //connected to taghelper to protect app login from cross site request forgery via hidden field where a value is added that later needs validation in our action
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await userManager.FindByNameAsync(loginModel.UserName); //if the username entered is found, the object is added to identityuser
                if (user != null)
                {
                    await signInManager.SignOutAsync(); //if user is already logged in, it gets signed out
                    if ((await signInManager.PasswordSignInAsync(user, loginModel.Password, false, false)).Succeeded) //checks password
                    {
                        if (await userManager.IsInRoleAsync(user, "Coordinator"))
                        {
                            return Redirect(loginModel?.ReturnUrl ?? "/Coordinator/StartCoordinator"); //cookie is created
                        }else if(await userManager.IsInRoleAsync(user, "Investigator"))
                        {
                            return Redirect(loginModel?.ReturnUrl ?? "/Investigator/StartInvestigator");
                        }
                        else if(await userManager.IsInRoleAsync(user, "Manager"))
                        {
                            return Redirect(loginModel?.ReturnUrl ?? "/Manager/StartManager");
                        }
                    }
                }
            }
            ModelState.AddModelError("", "Felaktigt användarnamn eller lösenord");
            return View(loginModel);
        }

        public async Task<RedirectResult> Logout(string returnUrl = "/")
        {
            await signInManager.SignOutAsync(); //if user is already logged in, it gets signed out
            return Redirect(returnUrl);
        }

        [AllowAnonymous]
        public ViewResult AccessDenied()
        {
            return View();
        }
    }
}
