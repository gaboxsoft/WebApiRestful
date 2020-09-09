using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebAppTest01
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
            services.AddControllers();
            
            #region 
            //-- Forzar HTTPS
            // Paso 1
            services.Configure<MvcOptions>
                (options=>{
                    options.Filters.Add(new RequireHttpsAttribute()); 
                });            
            #endregion //-- Forzar HTTPS
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            #region
            //--  Forza usar HTTPS
            // Requiere usar instalar Microsoft.AspNetCore.Rewrite
            // Paso 2
            // Cambiar en propiedades de proyecto, en debug habilitar SSL y copiar nuevo puerto
            var options = new RewriteOptions()
                .AddRedirectToHttps();
            #endregion //-- Forza usar HTTPS
        }
    }
}
