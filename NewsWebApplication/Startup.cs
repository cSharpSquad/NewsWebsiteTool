using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewDb;
using Validations;
using Exceptions;
using Microsoft.OpenApi.Models;
using System.Reflection;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

	public void ConfigureServices(IServiceCollection services)
	{
		services.AddDbContext<ApplicationDbContext>(options =>
			options.UseSqlServer(
				Configuration.GetConnectionString("DefaultConnection")));

		services.AddControllersWithViews();

		//services.AddRazorPages(); //*

		services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo
			{
				Title = "My Api",
				Version = "v1",
				Description = "A simple example ASP.NET Core Web API",
				Contact = new OpenApiContact
				{
					Name = "Your Name",
					Email = string.Empty,
					Url = new Uri("https://www.example.com/"),
				},
			});
			// Optional: Set the comments path for the Swagger JSON and UI.
			var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
			var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile); c.IncludeXmlComments(xmlPath);
		});
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();

			// Enable Swagger in Development Environment 
			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
				// Optional: Serve Swagger UI at a specific route (e.g., /swagger) 
				// c.RoutePrefix = "swagger";  
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

					// Log the exception or perform other actions  
				});
			});
			app.UseHsts();
		}

		// Common middleware for both Development and Production 
		app.UseHttpsRedirection();
		app.UseRouting();
		app.UseAuthorization();
		app.UseEndpoints(endpoints =>
		{
			endpoints.MapControllers();
		});
	}
	//   public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	//{
	//	if (env.IsDevelopment())
	//	{
	//		app.UseDeveloperExceptionPage();
	//	}
	//	else
	//	{
	//		app.UseExceptionHandler(errorApp =>
	//		{
	//			errorApp.Run(async context =>
	//			{
	//				context.Response.ContentType = "application/json";
	//				var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

	//				if (exceptionHandlerPathFeature?.Error is ResourceNotFoundException)
	//				{
	//					context.Response.StatusCode = StatusCodes.Status404NotFound;
	//					await context.Response.WriteAsync("{\"error\": \"Resource not found\"}");
	//				}
	//				else
	//				{
	//					context.Response.StatusCode = StatusCodes.Status500InternalServerError;
	//					await context.Response.WriteAsync("{\"error\": \"An unexpected error occurred.\"}");
	//				}

	//				// Log the exception or perform other actions 
	//			});
	//		});
	//		app.UseHsts();

	//	}

	//	// Other configurations... 
	//}
	//  {
	//      if (env.IsDevelopment())
	//      {
	//          app.UseDeveloperExceptionPage();
	//      }
	//      else
	//      {
	//          app.UseExceptionHandler("/Home/Error");
	//          app.UseHsts();
	//      }

	//      app.UseHttpsRedirection();
	//      app.UseStaticFiles();

	//      app.UseRouting();

	//app.UseMiddleware<NotFoundMiddleware>();

	//app.UseAuthorization();

	//      app.UseEndpoints(endpoints =>
	//      {
	//          endpoints.MapControllerRoute(
	//              name: "default",
	//              pattern: "{controller=Home}/{action=Index}/{id?}");
	//      });

	//      app.UseEndpoints(endpoints =>
	//      {
	//          endpoints.MapRazorPages();
	//      }); //*
	//  }
}
