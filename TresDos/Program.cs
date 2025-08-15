using TresDos.Application.Feature.Products.Commands;
using TresDos.Application.Mapping;
using TresDos.Application.Validators;
using TresDos.Core.Interfaces;
using TresDos.Feature.Users.Commands;
using TresDos.Infrastructure.Data;
using TresDos.Infrastructure.Repositories;
using TresDos.Application.Feature.Users.Queries;
using Serilog;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using TresDos.Services;
using TresDos.Application.Feature.DrawSettings.Queries;

var builder = WebApplication.CreateBuilder(args);

#region Configure Logging (Serilog)

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

#endregion

#region Configure Services

#region Database

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

#endregion

#region Dependency Injection (Repositories, Services)
builder.Services.AddHttpContextAccessor();

//Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITwoDRepository, TwoDRepository>();
builder.Services.AddScoped<IDrawSetingsRepository, DrawSetingsRepository>();

//Services
builder.Services.AddSingleton<ITokenService,TokenService>();
builder.Services.AddSingleton<ICacheDrawSettings, CacheDrawSettings>();
#endregion

#region MediatR

builder.Services.AddMediatR(cfg =>
{
    //Product
    cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly);

    //Users
    cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetAllUserQuery).Assembly);

    //Draw Settings
    cfg.RegisterServicesFromAssembly(typeof(GetAllDrawSettingsQuery).Assembly);
});

#endregion

#region AutoMapper

// AutoMapper registration (v15.0.1)
builder.Services.AddAutoMapper(
    typeof(ProductProfile), 
    typeof(UserProfile)
);
#endregion

#region FluentValidation

builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

#endregion

#region Authentication & Authorization

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthentication()
    .AddCookie(); // Optional, remove if not used

builder.Services.AddAuthorization();

#endregion

#region MVC, Views, and Session

builder.Services.AddControllersWithViews()
    .AddViewOptions(options =>
    {
        options.HtmlHelperOptions.ClientValidationEnabled = true;
    });

builder.Services.AddControllers();
builder.Services.AddSession();

#endregion

#region Swagger

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.DocInclusionPredicate((docName, apiDesc) =>
    {
        var controllerActionDescriptor = apiDesc.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
        return controllerActionDescriptor != null &&
               controllerActionDescriptor.ControllerTypeInfo
                   .GetCustomAttributes(typeof(ApiControllerAttribute), true).Length > 0;
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token.\nExample: Bearer abc123xyz"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

#endregion

#region HTTP Client

var apiBaseUrl = builder.Configuration.GetValue<string>("ApiSettings:BaseUrl");
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

#endregion

#region Other Config
builder.Configuration.GetValue<int>("BetSettings:TwoDMaxBet");
#endregion

#endregion

#region Configure Middleware Pipeline

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        options.RoutePrefix = "swagger";
    });

    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// app.UseMiddleware<ExceptionHandlingMiddleware>(); // Optional custom middleware

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();

#endregion
