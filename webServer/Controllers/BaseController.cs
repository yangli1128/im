using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace webServer.Controllers
{
    public class BaseController : Controller
    {
        protected string Appid { get; set; }
        //protected IServiceProvider ServiceProvider { get; }
        //public BaseController(IServiceProvider serviceProvider)
        //{
        //    ServiceProvider = serviceProvider;
        //    OnCreateProperties();
        //}
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var headers = context.HttpContext.Request.Headers;
            Appid = headers["appid"]; ;
            base.OnActionExecuting(context);
            OnCreateProperties(context);

        }
        protected virtual void OnCreateProperties(ActionExecutingContext context)
        {
            object controller = context.Controller;
            foreach (PropertyInfo declaredProperty in controller.GetType().GetTypeInfo().DeclaredProperties)
            {
                if (declaredProperty.CanWrite)
                {
                    declaredProperty.GetSetMethod(true).Invoke(controller, new object[1]
                    {
                        ActivatorUtilities.GetServiceOrCreateInstance(context.HttpContext.RequestServices, declaredProperty.PropertyType)
                    });
                }
            }
            foreach (var declaredProperty in controller.GetType().GetTypeInfo().DeclaredFields)
            {
                declaredProperty.SetValue(controller,
                        ActivatorUtilities.GetServiceOrCreateInstance(context.HttpContext.RequestServices, declaredProperty.FieldType)
                );
            }
        }

    }
}
