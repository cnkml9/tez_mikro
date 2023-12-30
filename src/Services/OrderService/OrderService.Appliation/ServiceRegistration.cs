using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using System;

namespace OrderService.Appliation
{
    public static class ServiceRegistration
    {
        readonly static IServiceCollection services;
    
        public static IServiceCollection AddApplicationRegistration(Type Program)
        {
            var assm = Assembly.GetExecutingAssembly();

            services.AddMediatR(assm);
            services.AddAutoMapper(assm);

            return services;
        }
    }
}
