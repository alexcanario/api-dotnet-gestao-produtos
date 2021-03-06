using DevIO.Api.Config;
using DevIO.Data.Context;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace DevIO.Api {
    public class Startup {
        private readonly string _connString;
        private readonly string _defaultConnectionString;
        public Startup(IConfiguration configuration) {
            _defaultConnectionString = @"Server=.; Database=MinhaApicompletaDb; User Id=sa; Password=Ca151867; MultipleActiveResultSets=True;";
            Configuration = configuration;
            _connString = string.IsNullOrEmpty(Configuration.GetConnectionString("DefaultWinConnectionString")) ? _defaultConnectionString : Configuration.GetConnectionString("DefaultWinConnectionString");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddAutoMapper(typeof(Startup));
            services.ResolveDependencies();
            services.AddDbContext<MeuDbContext>(options => {
                options.UseSqlServer(_connString);
            });

            services.AddControllers();

            
            //Alex Canario - 04/05/2022
            //Aula 5.8 - Desabilitar valida??o automatica do ModelState
            //Passo 1
            services.Configure<ApiBehaviorOptions>(options => {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddCors(options => {
                options.AddPolicy("Development", builder => 
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DevIO.Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DevIO.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseCors("Development");
        }
    }
}
