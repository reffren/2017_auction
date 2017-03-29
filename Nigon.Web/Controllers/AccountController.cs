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

        private UserActivation activation;

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

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                string activationCode = this.ActivationCode(); //get activation code
                activation = new UserActivation()
                {
                    UserName = model.UserName,
                    UserEmailAddress = model.Email,
                    Password = model.Password,
                    ActivationCode = activationCode
                };

                _repositoryUserActivation.SaveUserActivation(activation); //fill the temporary table
                 
                SendActivationEmail(model.UserName, model.Email, activationCode); //send activation email
                return RedirectToAction("ConfirmActivation", "Account");
            }
            return View(model);
        }

        public ActionResult ConfirmActivation()
        {
            return View();
        }
        public ActionResult Activation() 
        {
            if (RouteData.Values["id"] != null) //get the activation code
            {
                Guid activationCode = new Guid(RouteData.Values["id"].ToString());
                UserActivation userActivation = _repositoryUserActivation.UserActivations.FirstOrDefault(p => p.ActivationCode == activationCode.ToString());

                if (userActivation.UserName != null) // create new user (register)
                {
                    var createStatus = MembershipService.CreateUser(userActivation.UserName, userActivation.Password, userActivation.UserEmailAddress);

                    if (createStatus == MembershipCreateStatus.Success)
                    {
                        _repositoryUserActivation.DeleteUserActivation(userActivation); //delete the temporary value in table
                        FormsService.SignIn(userActivation.UserName, false /* createPersistentCookie */);
                    }
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
                else //  when user forgot password we check email address and send him to profile for changing password
                {
                    string userName = _repositoryUser.Users.Where(w => w.UserID == userActivation.UserID).Select(s => s.UserName).SingleOrDefault();
                    _repositoryUserActivation.DeleteUserActivation(userActivation); //delete the temporary value in table
                    FormsService.SignIn(userName, false /* createPersistentCookie */);
                }
            }

            return RedirectToAction("List", "Product");
        }
        private void SendActivationEmail(string userName, string userEmail, string activationCode)
        {

            using (MailMessage mm = new MailMessage("info@nigon.ru", userEmail))
            {
                mm.Subject = "Активация аккаунта";
                string body = "Здравствуйте " + userName + ",";
                body += "<br /><br />Пожалуйста, перейдите по этой ссылке для активации вашего аккаунта";
                body += "<br /><a href = '" + string.Format("{0}://{1}/account/activation/{2}", Request.Url.Scheme, Request.Url.Authority, activationCode) + "'>Click here to activate your account.</a>";
                body += "<br /><br />С уважением, nigon.ru";
                mm.Body = body;
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "mail.nigon.ru";
                smtp.EnableSsl = false;
                NetworkCredential NetworkCred = new NetworkCredential("info@nigon.ru", "******************");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);
            }
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPassword model)
        {
            if (ModelState.IsValid)
            {
                var UserData = _repositoryUser.Users.FirstOrDefault(f => f.UserEmailAddress == model.Email); //get email
                if (UserData.UserEmailAddress != null)
                {
                    string activationCode = this.ActivationCode(); //get activation code
                    activation = new UserActivation()
                    {
                        UserID = UserData.UserID,
                        ActivationCode = activationCode
                    };

                    _repositoryUserActivation.SaveUserActivation(activation); //fill the temporary table
                    this.SendActivationEmail(UserData.UserName, UserData.UserEmailAddress, activationCode); //send email for confirmation
                    return RedirectToAction("ConfirmActivation", "Account");
                }
                else
                {
                    ViewBag.Message = "С указанным логином нет зарегистрированных пользователей!";
                    return View();
                }
            }
            ViewBag.Message = "Ошибка! Пожалуйста, перезагрузите страницу и повторите ввод данных";
            return View();
        }

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
        public ActionResult ChangePassword(AccountModel model)
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

        private string ActivationCode()
        {
            return Guid.NewGuid().ToString();
        }
    }
}