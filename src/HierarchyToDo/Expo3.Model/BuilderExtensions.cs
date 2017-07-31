using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Expo3.Model
{
    public static class BuilderExtensions
    {
	    public static IServiceCollection AddExpo3Model<TConnection>(this IServiceCollection services, TConnection settings)
	    {
		    return services;
	    }

    }
}
