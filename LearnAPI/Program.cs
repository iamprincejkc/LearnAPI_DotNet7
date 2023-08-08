using AutoMapper;
using LearnAPI.Container;
using LearnAPI.DB;
using LearnAPI.Helper;
using LearnAPI.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Logging
builder.Host.UseSerilog((context, config)
    => config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddDbContext<LearnApiDbContext>(o => o.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")));

//Limit Netwrok Connections
builder.Services.AddRateLimiter(_ => _.AddFixedWindowLimiter(policyName: "fixedWindow", options =>
{
    options.Window = TimeSpan.FromSeconds(10);
    options.PermitLimit = 1;
    options.QueueLimit= 0;
    options.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
}).RejectionStatusCode=401);

//Map Model to Another Model
builder.Services.AddAutoMapper(typeof(AutoMapperHandler));

//Cors Policy for allowing different domains
builder.Services.AddCors(cors
    => cors.AddPolicy("CorsPolicy", build
    => build.WithOrigins("*").AllowAnyMethod().AllowAnyMethod()));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();

app.UseCors("CorsPolicy");

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
