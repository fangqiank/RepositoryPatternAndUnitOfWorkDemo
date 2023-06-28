using Microsoft.EntityFrameworkCore;
using UrlShort.Data;
using UrlShort.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(
    builder.Configuration.GetConnectionString("Defaults")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/shorturl", async (UrlDto urlDto, AppDbContext db, HttpContext ctx) =>
{
    if (!Uri.TryCreate(urlDto.Url, UriKind.Absolute, out var input))
        return Results.BadRequest("Invalid url has been provided");

    var random = new Random();

    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890@abcdefghijklmnopqrstuvwxyz";
    
    var randomString = new string(Enumerable.Repeat(chars, 8)
        .Select(x => x[random.Next(x.Length)]).ToArray());

    var sUrl = new UrlManagerment()
    {
        OriginalUrl = urlDto.Url,
        ShortUrl = randomString,
    };

    db.Urls.Add(sUrl);
   
    await db.SaveChangesAsync();

    var result = $"{ctx.Request.Scheme}://{ctx.Request.Host}/{sUrl.ShortUrl}";

    return Results.Ok(new UrlShortResponseDto
    {
        Url = result,
    });
});

app.MapFallback(async (AppDbContext db, HttpContext ctx) =>
{
    var path = ctx.Request.Path.ToUriComponent().Trim('/');

    var urlMatch = await db.Urls.FirstOrDefaultAsync(
        x => x.ShortUrl.Trim() == path.Trim());

    if (urlMatch == null)
        return Results.BadRequest("Invalid short url");

    return Results.Redirect(urlMatch.OriginalUrl);
});

app.Run();

