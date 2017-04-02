using Nigon.Data.Concrete;
using Nigon.Data.Abstract;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Nigon.Data.Entities;

namespace Nigon.Web.Infrastructure
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
            ninjectKernel.Bind<IProductRepository>().To<ProductRepository>();
            ninjectKernel.Bind<IProductViewRepository>().To<ProductViewRepository>();
            ninjectKernel.Bind<IImgProductRepository>().To<ImgProductRepository>();
            ninjectKernel.Bind<ICategoryRepository>().To<CategoryRepository>();
            ninjectKernel.Bind<IUserRepository>().To<UserRepository>();
            ninjectKernel.Bind<ISubCategoryRepository>().To<SubCategoryRepository>();
            ninjectKernel.Bind<IRateRepository>().To<RateRepository>();
        }
    }
}