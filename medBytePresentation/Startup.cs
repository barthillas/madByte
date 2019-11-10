using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using medBytePresentation.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using medBytePresentation.Controllers;

namespace medBytePresentation
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddControllersWithViews().AddRazorRuntimeCompilation(); 
            services.AddRazorPages();
            services.AddDistributedMemoryCache();
            services.AddSession(opts =>
            {
                
                opts.IdleTimeout = TimeSpan.FromMinutes(600);
            });
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.Cookie.Name = "mvcimplicit";
                options.LoginPath = new PathString("/Account/Login");
                options.AccessDeniedPath = new PathString("/Account/Login");

            });



            services.AddDistributedMemoryCache();


            services.AddHttpClient<IMedByteApiService, MedByteApiService>("MedByte", client =>
            {

                client.BaseAddress = new Uri("http://localhost:4000/apiv1/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");               

            
            });

            //.AddPolicyHandler(GetRetryPolicy())
            //.AddPolicyHandler(GetCircuitBreakerPolicy());
        
            
            //services.AddScoped<Post>(serviceProvider =>
            //{
            //    var accessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

            //    if (accessor.HttpContext.Items.TryGetValue(nameof(Post), out var post))
            //    {
            //        return post as Post;
            //    }
            //    else
            //    {
            //        return new GenericPost(); // we need to return something that is either a `Post` or derivative of `Post`.
            //    }
            //});
            services.AddMvc();


         
            services.AddSingleton<IMedByteApiService, MedByteApiService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
        
            app.UseStaticFiles();
          
            app.UseAuthentication();
            app.UseAuthorization();

            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }


}
