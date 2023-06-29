using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApiDemo.Data;
using MinimalApiDemo.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(
    builder.Configuration.GetConnectionString("Defaults")));

builder.Services.AddScoped<ItemRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var apiGroup = app.MapGroup("/todoItems");

apiGroup.MapGet("/", ([FromServices] ItemRepository repo) => repo.GetAll());

apiGroup.MapGet("/{id}", ([FromServices] ItemRepository repo, int id) =>
{
    return repo.GetById(id);
});

apiGroup.MapPost("/", ([FromServices] ItemRepository repo, Item newItem) =>
{
    repo.Add(newItem);
});

apiGroup.MapPut("/{id}", ([FromServices] ItemRepository repo, Item updItem) =>
{
    repo.Update(updItem);
});

apiGroup.MapDelete("/{id}", ([FromServices] ItemRepository repo, int id) =>
{
    repo.Delete(id);
});

app.Run();


