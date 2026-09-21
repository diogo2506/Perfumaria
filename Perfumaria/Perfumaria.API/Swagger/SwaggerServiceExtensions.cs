using Microsoft.OpenApi.Models;

namespace Perfumaria.API.Swagger;

/// <summary>
/// Configuração centralizada do Swagger/OpenAPI (CP3), incluindo metadados do
/// documento e a inclusão dos comentários XML gerados pelo projeto API.
/// </summary>
public static class SwaggerServiceExtensions
{
    public const string DocumentName = "v1";

    public static IServiceCollection AddPerfumariaSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        var title = configuration["Swagger:Title"] ?? "Perfumaria API";
        var description = configuration["Swagger:Description"]
            ?? "API REST para gestão de uma perfumaria: catálogo de produtos, estoque, " +
               "clientes, pedidos de venda e relacionamento com fornecedores e fabricantes.";
        var version = configuration["Swagger:Version"] ?? "v1";
        var contactName = configuration["Swagger:ContactName"] ?? "Perfumaria";

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(DocumentName, new OpenApiInfo
            {
                Title = title,
                Version = version,
                Description = description,
                Contact = new OpenApiContact { Name = contactName }
            });

            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            }
        });

        return services;
    }

    public static IApplicationBuilder UsePerfumariaSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint($"/swagger/{DocumentName}/swagger.json", "Perfumaria API v1");
            options.RoutePrefix = "swagger";
        });

        return app;
    }
}
