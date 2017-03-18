using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nigon.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(null,
                "",
                new { controller = "Product", action = "List", category = (string)null, subcategory = (string)null, page = 1 }
                );

            routes.MapRoute(
                null,
                "Page{page}",
                new { controller = "Product", action = "List", category = (string)null, subcategory = (string)null },
                new { page = @"\d+" }

            );

            routes.MapRoute(null,
                "catalog/{subcategory}",
                new { controller = "Product", action = "List", page = 1 }
                );

            routes.MapRoute(null,
               "catalog/{subcategory}/{productID}",
               new { controller = "Product", action = "ProductView", productID = (string)null },
               new { productID = @"\d+" }
               );

            routes.MapRoute(null,
                "catalog/{subcategory}/Page{page}",
                new { controller = "Product", action = "List" },
                new { page = @"\d+" }
            );

            routes.MapRoute(null, "{controller}/{action}");
            routes.LowercaseUrls = true;
        }
    }
}

