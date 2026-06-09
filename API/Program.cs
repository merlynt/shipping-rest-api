using System.Reflection;
using System.Text;
using API.Swagger;
using Application.Interfaces;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("ENTORNO = " + builder.Environment.EnvironmentName);
Console.WriteLine("CONEXION = " +
    builder.Configuration.GetConnectionString("CadenaConexion"));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaConexion")));

// Registro de servicios existentes
builder.Services.AddScoped<Domain.Interfaces.ITrackingService, Application.Services.TrackingService>();
builder.Services.AddScoped<Domain.Interfaces.IShipmentRepository, Infrastructure.Repositories.ShipmentRepository>();
builder.Services.AddScoped<Domain.Interfaces.IRecipientRepository, Infrastructure.Repositories.RecipientRepository>();
builder.Services.AddScoped<Domain.Interfaces.IUsuarioRepository, Infrastructure.Repositories.UsuarioRepository>();
builder.Services.AddScoped<Domain.Interfaces.IAdminRepository, Infrastructure.Repositories.AdminRepository>();
builder.Services.AddScoped<Application.Interfaces.IAdminService, Application.Services.AdminService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<Domain.Interfaces.IUserContext, Infrastructure.Services.UserContext>();
builder.Services.AddScoped<Domain.Interfaces.IDriverRepository, Infrastructure.Repositories.DriverRepository>();

builder.Services.AddScoped<Domain.Interfaces.ITokenService, Infrastructure.Security.TokenService>();

builder.Services.AddScoped<Domain.Interfaces.ICompanyRepository, Infrastructure.Repositories.CompanyRepository>();
builder.Services.AddScoped<Application.Interfaces.ICompanyService, Application.Services.CompanyService>();

builder.Services.AddScoped<Application.Interfaces.IShipmentService, Application.Services.ShipmentService>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SoloAdminMaster", policy =>
    {
        // Exige que el token tenga el claim "esMaster" con valor "true"
        policy.RequireClaim("esMaster", "true", "True");
    });
});


builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            return new UnprocessableEntityObjectResult(context.ModelState);
        };
    });
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sistema de Gestión de Envíos - API",
        Version = "v1",
        Description = "Backend para el control de logística, administradores, empresas y pilotos."
    });


    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization. Escribe 'Bearer {tu_token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{ Type = ReferenceType.SecurityScheme, Id = "Bearer"}
            },
            new string[]{}
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sistema Envíos V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();