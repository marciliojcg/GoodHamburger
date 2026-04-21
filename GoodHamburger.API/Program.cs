using GoodHamburger.Application.Ports;
using GoodHamburger.Application.UseCases;
using GoodHamburger.Domain.Exceptions;
using GoodHamburger.Infrastructure.Data;
using GoodHamburger.Infrastructure.Repositories;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddSingleton(new DapperContext(
    builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<ICardapioRepository, CardapioRepository>();

// Use Cases
builder.Services.AddScoped<PedidoUseCases>();
builder.Services.AddScoped<CardapioUseCases>();

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Global exception handler
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        if (exception is NotFoundException)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsJsonAsync(new { erro = exception.Message });
        }
        else if (exception is DomainException)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new { erro = exception.Message });
        }
        else
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new { erro = "Erro interno do servidor" });
        }
    });
});

app.Run();