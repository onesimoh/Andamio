using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Andamio.Security
{
    public enum EnvironmentContext
    {
        Undefined = 0,
        Development = 1,
        Test = 2,
        Preview = 3,
        Production = 4,
    }

    public static class EnvironmentContextExtensions
    {
        public static bool IsDefined(this EnvironmentContext environment)
        {
            return environment != EnvironmentContext.Undefined;
        }
        public static bool IsDevelopment(this EnvironmentContext environment)
        {
            return environment == EnvironmentContext.Development;
        }    
        public static bool IsTest(this EnvironmentContext environment)
        {
            return environment == EnvironmentContext.Test;
        }
        public static bool IsPreview(this EnvironmentContext environment)
        {
            return environment == EnvironmentContext.Preview;
        }
        public static bool IsProduction(this EnvironmentContext environment)
        {
            return environment == EnvironmentContext.Production;
        }
        public static bool IsRelease(this EnvironmentContext environment)
        {
            return environment.IsProduction() || environment.IsPreview();
        }
    }
}
