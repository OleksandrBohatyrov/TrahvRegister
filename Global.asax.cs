using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TrajvRegister10.Models;

namespace TrajvRegister10
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            Database.SetInitializer(new ApplicationDbInitializer());

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            HttpCookie langCookie = HttpContext.Current.Request.Cookies["lang"];
            if (langCookie != null && !string.IsNullOrEmpty(langCookie.Value))
            {
                var cultureInfo = new System.Globalization.CultureInfo(langCookie.Value);
                System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
                System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;
            }
            else
            {
                // ������������� ���� �� ���������
                var cultureInfo = new System.Globalization.CultureInfo("en");
                System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
                System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;
            }
        }

    }
}
