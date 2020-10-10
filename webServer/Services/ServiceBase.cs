using CS.Base.Interface;
using webServer.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace CS.Base
{
    public abstract class ServiceBase : ITransient
    {
        private Lazy<imdbContext> dbContext;
        protected imdbContext db => dbContext.Value;
        protected IServiceProvider ServiceProvider { get; }
        public ServiceBase(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            dbContext = new Lazy<imdbContext>(() => ServiceProvider.GetService<imdbContext>());
            OnCreateProperties();
        }

        protected virtual void OnCreateProperties()
        {
            object controller = this;
            //foreach (PropertyInfo declaredProperty in controller.GetType().GetTypeInfo().DeclaredProperties)
            //{
            //    if (declaredProperty.CanWrite)
            //    {
            //        declaredProperty.GetSetMethod(true).Invoke(controller, new object[1]
            //        {
            //            ActivatorUtilities.GetServiceOrCreateInstance(ServiceProvider, declaredProperty.PropertyType)
            //        });
            //    }
            //}
            foreach (var declaredProperty in controller.GetType().GetTypeInfo().DeclaredFields)
            {
                declaredProperty.SetValue(controller, ActivatorUtilities.GetServiceOrCreateInstance(ServiceProvider, declaredProperty.FieldType));
            }
        }
    }
}
