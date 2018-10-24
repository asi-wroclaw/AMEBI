using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AMEBI.Domain.Configs;
using AMEBI.Domain.EF;
using AMEBI.Domain.LDAP;
using AMEBI.Domain.Model;
using AMEBI.Domain.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AMEBI.Mvc
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddSingleton<IJwtService, JwtService>();

            services.AddDbContext<DatabaseContext>(options =>
                options.UseInMemoryDatabase("db"));

            services.AddAuthorization(options =>
            {
                options.AddPolicy("user", policy => policy
                    .RequireAuthenticatedUser().RequireClaim(ClaimTypes.Role, "user")
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                );

                options.AddPolicy("admin", policy => policy
                    .RequireAuthenticatedUser().RequireClaim(ClaimTypes.Role, "admin")
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                );
            });

            services.AddAuthentication(options =>  
            {  
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;  
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; 
            })  
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["IssuerSigningKey"])),  

                    ValidateIssuer = true,  
                    ValidIssuer = Configuration["ValidIssuer"],

                    ValidateAudience = false,   
                    ValidateLifetime = true,  
                   
                    ClockSkew = TimeSpan.Zero 
                };
            });

            services.AddOptions(); 
            services.Configure<LdapConfig>(Configuration);
            services.Configure<AppConfig>(Configuration);

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
