using AutoMapper;
using LearnAPI.Container;
using LearnAPI.DB;
using LearnAPI.Helper;
using LearnAPI.Model;
using LearnAPI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//Scaffold
//"Scaffold-DbContext 'Name=DefaultConnection' Microsoft.EntityFrameworkCore.SqlServer -OutputDir DB/Models -context LearnAPIDbContext -f -contextDir DB -DataAnnotations"

// Add services to the container.

//Logging
builder.Host.UseSerilog((context, config)
    => config.ReadFrom.Configuration(context.Configuration));

builder.Services.Configure<JwtSettingsModel>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddTransient<IRefreshHandler, RefreshHandler>();
builder.Services.AddDbContext<LearnAPIDbContext>(o => o.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")));

//Basic Authentication
//builder.Services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication",null);

//JWT Authentication
builder.Services.AddAuthentication(item =>
{
    item.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    item.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(item =>
{
    item.RequireHttpsMetadata = true;
    item.SaveToken = true;
    item.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtSettings:SecurityKey"))),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero,
    };
});

//Limit Netwrok Connections
builder.Services.AddRateLimiter(_ => _.AddFixedWindowLimiter(policyName: "fixedWindow", options =>
{
    options.Window = TimeSpan.FromSeconds(10);
    options.PermitLimit = 1;
    options.QueueLimit = 0;
    options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
}).RejectionStatusCode = 401);

//Map Model to Another Model
builder.Services.AddAutoMapper(typeof(AutoMapperHandler));

//Cors Policy for allowing different domains
builder.Services.AddCors(cors
    => cors.AddPolicy("CorsPolicy", build
    => build.WithOrigins("*").AllowAnyMethod().AllowAnyMethod()));

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Learn API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});


var app = builder.Build();


app.MapGet("/api/Minimal", () => "JKC");
app.MapGet("/api/Welcome", (string name) => "Welcome " + name).WithOpenApi(op =>
{
    var param = op.Parameters[0];
    param.Description = "Enter you name";
    return op;
});
app.MapGet("/api/GetCustomers", async (LearnAPIDbContext db) =>
{
    return await db.TblCustomers.ToListAsync();
});

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseStaticFiles();

app.UseRateLimiter();

app.UseCors("CorsPolicy");

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
