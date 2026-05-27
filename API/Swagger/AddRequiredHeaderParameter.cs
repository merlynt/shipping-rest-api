using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace API.Swagger 
{
    public class AddRequiredHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {

            var controllerName = context.ApiDescription.ActionDescriptor.RouteValues["controller"];

            // Solo mostrar el header si estamos en Shipments o Recipients
            if (controllerName != "Shipments" && controllerName != "Recipients")
            {
                return; 
            }

            operation.Parameters ??= new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-Empresa-Id",
                In = ParameterLocation.Header,
                Required = false,
                Description = "ID de la empresa (Opcional)",
                Schema = new OpenApiSchema { Type = "integer", Default = new Microsoft.OpenApi.Any.OpenApiInteger(5) }
            });
        }
    }
}