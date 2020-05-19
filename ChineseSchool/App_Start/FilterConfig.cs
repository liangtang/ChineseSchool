using System;
using System.Web;
using System.Web.Mvc;

namespace ChineseSchool
{
    public class RequireSecureConnectionFilter : RequireHttpsAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
           
            if (filterContext.HttpContext.Request.IsLocal)
            {
                return;
            }

            base.OnAuthorization(filterContext);
        }
    }
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new RequireSecureConnectionFilter());
        }
    }
}
