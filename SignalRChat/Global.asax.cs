using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SignalRChat.DAL;
using SignalRChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SignalRChat
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            IdentityContext db = new IdentityContext();
            RoleStore<ApplicationRole> roleStore = new RoleStore<ApplicationRole>(db);
            RoleManager<ApplicationRole> roleManager = new RoleManager<ApplicationRole>(roleStore);
            if (!roleManager.RoleExists("Representative"))
            {
                ApplicationRole adminRole = new ApplicationRole("Representative", "Müşteri Temsilcisi");
                roleManager.Create(adminRole);
            }
            if (!roleManager.RoleExists("Customer"))
            {
                ApplicationRole userRole = new ApplicationRole("Customer", "Müşteri");
                roleManager.Create(userRole);
            }
        }
    }
}
