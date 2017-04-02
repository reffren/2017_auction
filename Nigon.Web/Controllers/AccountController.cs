using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using Nigon.Data.Concrete;
using Nigon.Data.Entities;
using Nigon.Web.Identity;
using Nigon.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Nigon.Web.Controllers
{
    public class AccountController : Controller
    {
        string errorMessage = "";
        public AccountController()
            : this(new UserManager<User>(new UserStore<User>(new EFContext())))
        {
        }

        public AccountController(UserManager<User> userManager)
        {
            UserManager = userManager;
            Token();
        }

        public void Token()
        {
            var provider = new MachineKeyProtectionProvider();
            UserManager.UserTokenProvider = new DataProtectorTokenProvider<User>(provider.Create("ASP.NET Identity"));
        }
        public UserManager<User> UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Неккоректное имя пользователя или e-mail.");
                }
            }

            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("List", "Product");
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                };
                var emailResult = await UserManager.FindByEmailAsync(model.Email); //check email
                var result = await UserManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    errorMessage = "Пользователь с таким именем уже существует";
                }
                else if (emailResult != null)
                {
                    errorMessage = "Пользователь с таким email уже существует";
                }

                if (result.Succeeded && emailResult == null)
                {
                    string emailConfirmationCode = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id); //generate token for email
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = emailConfirmationCode }, protocol: Request.Url.Scheme);
                    string sFormat = string.Format("Здравствуйте {0}!<BR/>Благодарим Вас за регистрацию, пожалуйста перейдите по <a href=\"{1}\" title=\"Регистрация в Интернет-аукционе Nigon.ru\">ссылке</a>, чтобы завершить вашу решистрацию.<br/><br/> Если вы не отправляли запрос на восстановление пароля, просто проигнорируйте это письмо.<br/><br/> Это письмо сформировано автоматически, не отвечайте на него. С уважением, Интернет-аукцион Nigon.ru", user.UserName, callbackUrl);
                    string email = user.Email;
                    string subject = "Регистрация в Интернет-аукционе Nigon.ru";
                    SendEmail(sFormat, email, subject);

                    return RedirectToAction("InfoConfirmEmail", "Account", new { Email = user.Email });
                }
                else
                {
                    AddErrors(errorMessage);
                }
            }
            return View(model);
        }

        public ActionResult InfoConfirmEmail()
        {
            return View();
        }

        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }



        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await this.UserManager.FindByEmailAsync(model.Email);

                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    return View("ForgotPasswordConfirmation");
                }

                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id); //generate token for reset password
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                TempData["ViewBagLink"] = callbackUrl;
                string sFormat = string.Format("Здравствуйте {0}!<br/>Это письмо было выслано в ответ на запрос восстановления пароля в интернет-аукционе Nigon.ru. Для восстановления пароля перейдите по <a href=\"{1}\" title=\"Запрос на смену пароля\">ссылке</a> и следуйте инструкциям.<br/><br/> Если вы не отправляли запрос на восстановление пароля, просто проигнорируйте это письмо.<br/><br/> Это письмо сформировано автоматически, не отвечайте на него. С уважением, Интернет-аукцион Nigon.ru", user.UserName, callbackUrl);
                string email = user.Email;
                string subject = "Запрос на смену пароля в интернет-аукционе Nigon.ru";
                SendEmail(sFormat, email, subject);
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            ViewBag.Link = TempData["ViewBagLink"];
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await UserManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            else
            {
                errorMessage = "Ошибка! Пожалуйста проверьте правильность введенных данных";
            }
            AddErrors(errorMessage);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        public void SendEmail(string sFormat, string Email, string subject)
        {
            System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(new System.Net.Mail.MailAddress("info@nigon.ru", "Интернет-аукцион Nigon.ru"), new System.Net.Mail.MailAddress(Email));
            m.Subject = subject;
            m.Body = sFormat;
            m.IsBodyHtml = true;
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("mail.nigon.ru");
            smtp.Credentials = new System.Net.NetworkCredential("info@nigon.ru", "******************");
            smtp.EnableSsl = false;
            smtp.Send(m);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(User user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(string error)
        {
            ModelState.AddModelError("", error);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("List", "Product");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

        }
    }
}