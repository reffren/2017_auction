using Nigon.WebUI.Infrastructure.Abstract;
using Nigon.WebUI.Infrastructure.Concrete;
using Nigon.WebUI.Models.AccountModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace Nigon.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private IFormsAuthenticationService FormsService { get; set; }
        private IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountService(); }

            base.Initialize(requestContext);
        }

        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    FormsService.SignIn(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl ?? Url.Action("List", "Product"));
                    }
                    return RedirectToAction("RedirectToDefault");
                }
                ModelState.AddModelError("", "Неверно введены Имя пользователя или Пароль.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff
        public ActionResult RedirectToDefault()
        {

            String[] roles = Roles.GetRolesForUser();
            if (roles.Contains("Administrator"))
            {
                return RedirectToAction("Index", "Admin");
            }
            else if (roles.Contains("User"))
            {
                return RedirectToAction("List", "Product");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("List", "Product");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                var createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("List", "Product");
                }
                ModelState.AddModelError("", ErrorCodeToString(createStatus));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        // ChangePassword method not implemented in CustomMembershipProvider.cs
        // Feel free to update!

        //
        // POST: /Account/ChangePassword

        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Имя пользователя уже существует. Пожалуйста введите другое имя.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Данный e-mail уже существует. Пожалуйста, введите другой e-mail адрес.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Пароль введен неверно. Пожалуйста, проверьте правильность введеного вами пароля.";

                case MembershipCreateStatus.InvalidEmail:
                    return "e-mail введен неверно. Пожалуйста, проверьте правильность введеного вами e-mail адреса.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "Ответ на вопрос введен неверно. Пожалуйста, проверьте правильность введеного вами ответа на вопрос.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "Вопрос введен неверно. Пожалуйста, проверьте правильность введеного вами вопроса.";

                case MembershipCreateStatus.InvalidUserName:
                    return "Имя пользователя введено неверно. Пожалуйста, проверьте правильность введеного вами вопроса.";

                case MembershipCreateStatus.ProviderError:
                    return "Произошла ошибка. Пожалуйста, проверьте ваши данные. Если проблема повторяется, пожалуйста обратитесь к администратору.";

                case MembershipCreateStatus.UserRejected:
                    return "Запрос отменен. Пожалуйста, проверьте ваши данные. Если проблема повторяется, пожалуйста обратитесь к администратору.";

                default:
                    return "Произошла необработанная ошибка. Пожалуйста, проверьте ваши данные. Если проблема повторяется, пожалуйста обратитесь к администратору.";
            }
        }
        #endregion

    }
}