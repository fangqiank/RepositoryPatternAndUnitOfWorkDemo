using Hangfire;
using Hangfire.Storage.SQLite;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RepositoryPatternAndUnitOfWork.Core.IConfiguration;
using RepositoryPatternAndUnitOfWork.Data;
using RepositoryPatternAndUnitOfWork.Services.Background;
using RepositoryPatternAndUnitOfWork.Services.Health;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(
    builder.Configuration.GetConnectionString("Defaults")));

builder.Services.AddHangfire(config => config.UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSQLiteStorage(builder.Configuration.GetConnectionString("Defaults")));

builder.Services.AddHangfireServer();

//builder.Services.AddEntityFrameworkNpgsql()
//    .AddDbContext<ApplicationDbContext>(option =>
//    {
//        option.UseNpgsql(builder.Configuration.GetConnectionString("Defaults"));
//    });

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowOrigin", builder => builder.AllowAnyOrigin().AllowAnyMethod());
//});

builder.Services.AddHealthChecks()
    .AddCheck<ApiHealthCheck>("MyApiChecks", tags: new string[]
    {
        "MyApi"
    } );

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "App",
            Version = "v1"
        });

        c.AddSecurityDefinition("Bearer", 
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            //Scheme = JwtBearerDefaults.AuthenticationScheme.ToLowerInvariant(),
            Scheme = "Bearer",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Name = "Authorization",
            BearerFormat = "JWT",
            Description = "JWT Autherization header using Bearer scheme"
        });

        //c.OperationFilter<AuthResponsesOperationFilter>();
        
    });

builder.Services.AddTransient<IServiceManagement, ServiceManagement>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var key = Encoding.ASCII.GetBytes(builder.Configuration
            .GetSection("JwtConfig:Secret").Value!);

var tokenValidationParams = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    RequireExpirationTime = false,
    //ClockSkew = TimeSpan.Zero,
};

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(jwt => {
        jwt.SaveToken = true;
        jwt.TokenValidationParameters = tokenValidationParams;
    });

builder.Services.AddSingleton(tokenValidationParams);

//builder.Services.AddDefaultIdentity<IdentityUser>
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("AccessDbPolicy", 
        policy => policy.RequireClaim("dbaccess"));
});

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseHangfireDashboard();
app.MapHangfireDashboard();

RecurringJob.AddOrUpdate<IServiceManagement>(x => x.SyncData(), "0 * * ? * *");

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
