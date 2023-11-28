using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using System.IO;
using NewDb; // Make sure this is the correct namespace for your ApplicationDbContext 
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Exceptions;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services, builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddApiVersioning(options =>
{
	options.DefaultApiVersion = new ApiVersion(1, 0); // Default version is 1.0 
	options.AssumeDefaultVersionWhenUnspecified = true; // If no version is specified, default is used 
	options.ReportApiVersions = true; // Report which versions are supported 
									  // Additional configuration here if needed 
});

var app = builder.Build();

Configure(app, app.Environment);

app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
	// Add services to the container. 
	services.AddControllersWithViews();
	services.AddRazorPages();

	services.AddControllers();

	services.AddApiVersioning(options =>
	{
		options.DefaultApiVersion = new ApiVersion(1, 0);
		options.AssumeDefaultVersionWhenUnspecified = true;
		options.ReportApiVersions = true;
	});

	services.AddDbContext<ApplicationDbContext>(options =>
		options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

	services.AddSwaggerGen(c =>
	{
		c.SwaggerDoc("v1", new OpenApiInfo
		{
			Title = "My API",
			Version = "v1",
			Description = "A simple example ASP.NET Core Web API",
			Contact = new OpenApiContact
			{
				Name = "Your Name",
				Email = string.Empty,
				Url = new Uri("https://www.example.com/"),
			},
		});

		var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
		var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
		c.IncludeXmlComments(xmlPath);
	});

	// Add any other services here 
}

void Configure(WebApplication app, IWebHostEnvironment env)
{
	// Configure the HTTP request pipeline. 
	if (env.IsDevelopment())
	{
		app.UseDeveloperExceptionPage();
		app.UseSwagger();
		app.UseSwaggerUI(c =>
		{
			c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
			c.RoutePrefix = "swagger";
		});
	}
	else
	{
		app.UseExceptionHandler(errorApp =>
		{
			errorApp.Run(async context =>
			{
				context.Response.ContentType = "application/json";
				var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

				if (exceptionHandlerPathFeature?.Error is ResourceNotFoundException)
				{
					context.Response.StatusCode = StatusCodes.Status404NotFound;
					await context.Response.WriteAsync("{\"error\": \"Resource not found\"}");
				}
				else
				{
					context.Response.StatusCode = StatusCodes.Status500InternalServerError;
					await context.Response.WriteAsync("{\"error\": \"An unexpected error occurred.\"}");
				}
			});
		});
		app.UseHsts();
	}

	app.UseHttpsRedirection();
	app.UseStaticFiles();

	app.UseRouting();

	app.UseAuthorization();

	app.MapControllers();
	app.MapRazorPages();
}