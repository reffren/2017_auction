using Nigon.WebUI.Infrastructure.Abstract;
using Nigon.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nigon.WebUI.Controllers
{
    public class AccountController : Controller
    {
        IAuthProvider authProvider;
        public AccountController(IAuthProvider auth)
        {
            authProvider = auth;
        }
        public ViewResult LogOn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LogOn(LogOnViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (authProvider.Authenticate(model.Username, model.Password))
                {
                    return Redirect(returnUrl ?? Url.Action("List", "Product"));
                }
                else
                {
                    ModelState.AddModelError("", "Неккоректно введены Имя пользователя или Пароль!");
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

    }
}
