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
    public class BaseController : ControllerBase
    {
        protected IServiceProvider ServiceProvider { get; }
        public BaseController(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            OnCreateProperties();
        }
        protected virtual void OnCreateProperties()
        {
            object controller = this;
            foreach (PropertyInfo declaredProperty in controller.GetType().GetTypeInfo().DeclaredProperties)
            {
                if (declaredProperty.CanWrite)
                {
                    declaredProperty.GetSetMethod(true).Invoke(controller, new object[1]
                    {
                        ActivatorUtilities.GetServiceOrCreateInstance(ServiceProvider, declaredProperty.PropertyType)
                    });
                }
            }
            foreach (var declaredProperty in controller.GetType().GetTypeInfo().DeclaredFields)
            {
                declaredProperty.SetValue(controller,

                        ActivatorUtilities.GetServiceOrCreateInstance(ServiceProvider, declaredProperty.FieldType)

                );
            }
        }

    }
}
