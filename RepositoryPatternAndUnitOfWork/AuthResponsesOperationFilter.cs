using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class AuthResponsesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var attr = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
            .Union(context.MethodInfo.GetCustomAttributes(true));
            //.OfType<AuthorizeAttribute>();

        if (attr.OfType<IAllowAnonymous>().Any())
        {
            //operation.Responses.Add("401", new OpenApiResponse
            //{
            //    Description = "Unauthorized"
            //});
            return;

            var authAttr = attr.OfType<IAuthorizeData>();

            if (authAttr.Any()) 
            {
                operation.Responses["401"] = new OpenApiResponse
                {
                    Description = "Unauthorized"
                };

                if(authAttr.Any(a => !string.IsNullOrWhiteSpace(a.Roles) 
                    || !string.IsNullOrWhiteSpace(a.Policy)))
                {
                    operation.Responses["403"] = new OpenApiResponse
                    {
                        Description = "Forbidden"
                    };
                }

                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Description="Adds token to header",
                            Name="Authorization",
                            Type=SecuritySchemeType.Http,
                            In=ParameterLocation.Header,
                            Scheme = JwtBearerDefaults.AuthenticationScheme,
                            Reference = new OpenApiReference
                            {
                                Type= ReferenceType.SecurityScheme,
                                Id=JwtBearerDefaults.AuthenticationScheme
                            }
                        },new List<string>()
                    }
                });
            }

        }
    }
}