using Nigon.Data.Abstract;
using Nigon.Data.Entities;
using Nigon.Web.Infrastructure;
using Nigon.Web.Infrastructure.Abstract;
using Nigon.Web.Infrastructure.Concrete;
using Nigon.Web.Models.AccountModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace Nigon.Web.Controllers
{
    public class AccountController : Controller
    {
        private IFormsAuthenticationService FormsService { get; set; }
        private IAccountService MembershipService { get; set; }
        private IUserActivationRepository _repositoryUserActivation;
        private IUserRepository _repositoryUser;

        public AccountController(IUserActivationRepository userActivation, IUserRepository user)
        {
            _repositoryUserActivation = userActivation;
            _repositoryUser = user;
        }

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
                    SendActivationEmail(model.UserName, model.Email);
                    return RedirectToAction("ConfirmActivation", "Account");
                }
                ModelState.AddModelError("", ErrorCodeToString(createStatus));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult ConfirmActivation()
        {
            return View();
        }
        public ActionResult Activation()
        {
            ViewBag.Message = "Ошибка! Неверный код активации!";
            if (RouteData.Values["id"] != null)
            {
                Guid activationCode = new Guid(RouteData.Values["id"].ToString());
                UserActivation userActivation = _repositoryUserActivation.UserActivations.Where(p => p.ActivationCode == activationCode.ToString()).FirstOrDefault();
                if (userActivation != null)
                {
                    _repositoryUserActivation.DeleteUserActivation(userActivation);
                    return RedirectToAction("List", "Product");
                }
            }

            return View();
        }
        private void SendActivationEmail(string userName, string userEmail)
        {
            Guid activationCode = Guid.NewGuid();
            UserActivation userActivation = new UserActivation()
            {
                UserID = _repositoryUser.Users.Where(f => f.UserName == userName).Select(s => s.UserID).Single(),
                ActivationCode = activationCode.ToString(),
            };

            _repositoryUserActivation.SaveUserActivation(userActivation);

            using (MailMessage mm = new MailMessage("info@nigon.ru", userEmail))
            {
                mm.Subject = "Account Activation";
                string body = "Здравствуйте " + userName + ",";
                body += "<br /><br />Пожалуйста, перейдите по этой ссылке для активации вашего аккаунта";
                body += "<br /><a href = '" + string.Format("{0}://{1}/account/activation/{2}", Request.Url.Scheme, Request.Url.Authority, activationCode) + "'>Click here to activate your account.</a>";
                body += "<br /><br />С уважением, nigon.ru";
                mm.Body = body;
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "mail.nigon.ru";
                smtp.EnableSsl = false;
                NetworkCredential NetworkCred = new NetworkCredential("info@nigon.ru", "*************");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);
            }
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