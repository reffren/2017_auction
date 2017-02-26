using Nigon.Domain.Abstract;
using Nigon.Domain.Concrete;
using Nigon.WebUI.Infrastructure.Abstract;
using Nigon.WebUI.Infrastructure.Concrete;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nigon.WebUI.Infrastructure
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        private IKernel ninjectKernel;

        public NinjectControllerFactory()
        {
            ninjectKernel = new StandardKernel();
            AddBindings();
        }

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            return controllerType == null ? null : (IController)ninjectKernel.Get(controllerType);
        }

        private void AddBindings()
        {
            ninjectKernel.Bind<IProductsRepository>().To<EFDbProductsRepository>();
           // EmailSettings emailSettings = new EmailSettings { WriteAsFile = bool.Parse(ConfigurationManager.AppSettings["Email.WriteAsFile"] ?? "false") };
          //  ninjectKernel.Bind<IOrderProcessor>().To<EmailOrderProcessor>().WithConstructorArgument("settings", emailSettings);
            ninjectKernel.Bind<IAuthProvider>().To<FormsAuthProvider>();
            ninjectKernel.Bind<IMembershipService>().To<AccountMembershipService>();
            ninjectKernel.Bind<IFormsAuthenticationService>().To<FormsAuthenticationService>();
            ninjectKernel.Bind<IProductViewRepository>().To<EFDbProductViewRepository>();
            ninjectKernel.Bind<IImgProductRepository>().To<EFDbImgProductRepository>();
            ninjectKernel.Bind<ICategoryRepository>().To<EFDbCategoryRepository>();
            ninjectKernel.Bind<IUserRepository>().To<EFDbUserRepository>();
            ninjectKernel.Bind<ISubCategoryRepository>().To <EFDbSubCategoryRepository>();
            ninjectKernel.Bind<IRateRepository>().To<EFDbRateRepository>();
        }
    }
}