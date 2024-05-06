using DemoCleanArchitecture.Infrastructure.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoCleanArchitecture.Infrastructure.Attribute
{
    public class JwtAuthorizeAttribute : TypeFilterAttribute
    {

        public JwtAuthorizeAttribute(
            params string[] roles
            )
            : base(typeof(JwtAuthorizeFilter))
        {
            Arguments = new object[] { roles };
        }
    }
}
