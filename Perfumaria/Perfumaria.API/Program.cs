using Perfumaria.API.Exceptions;
using Perfumaria.API.HealthChecks;
using Perfumaria.API.Swagger;
using Perfumaria.Application.Interfaces.Repositories;
using Perfumaria.Application.Services;
using Perfumaria.Infrastructure.Data;
using Perfumaria.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Persistência (herdada do CP2)
builder.Services.AddDbContext<PerfumariaDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositório genérico (CP3) — disponível para qualquer entidade que implemente IEntity<TKey>
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

// Repositórios específicos por agregado (convivem com o genérico quando há consultas extras)
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IEstoqueRepository, EstoqueRepository>();

// Serviços de aplicação (CP4) — orquestram repositórios + regras de Domain,
// tornando os fluxos de escrita testáveis com mocks (Perfumaria.Application.Tests).
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();

// Tratamento global de exceções (CP3) — RFC 7807 / ProblemDetails
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Health checks (CP4)
builder.Services.AddPerfumariaHealthChecks(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddPerfumariaSwagger(builder.Configuration);

var app = builder.Build();

// O tratamento global de exceções deve vir antes de MapControllers/Swagger.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UsePerfumariaSwagger();

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<PerfumariaDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();
app.MapControllers();

// Único endpoint de health check (CP4): processo (self) + banco (database) + URL
// externa opcional (external-site), com writer JSON e status HTTP coerentes.
app.UsePerfumariaHealthChecks();

app.Run();

/// <summary>
/// Classe parcial exposta apenas para permitir testes de integração (ex.: WebApplicationFactory)
/// referenciarem o entry point da API.
/// </summary>
public partial class Program { }
