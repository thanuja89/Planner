using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Planner.Api.Services;
using Planner.Domain;
using Planner.Domain.Entities;
using Planner.Domain.Repositories;
using Planner.Domain.Repositories.Interfaces;
using Planner.Domain.UnitOfWork;
using Serilog;
using System.IO;
using System.Text;

namespace Planner.Api
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;

        public Startup(IConfiguration configuration
            , IHostingEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<PlannerDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("PlanningDb"), b => b.MigrationsAssembly("Planner.Api")));

            services.AddDefaultIdentity<ApplicationUser>(opt => 
            {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 4; // For testing
                opt.Password.RequireLowercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.User.RequireUniqueEmail = true;

                opt.Tokens.EmailConfirmationTokenProvider = "NumericTokenProvider";
            })
                .AddEntityFrameworkStores<PlannerDbContext>()
                .AddTokenProvider<NumericTokenProvider>("NumericTokenProvider");

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                .Build());
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(options =>
                 {
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuer = true,
                         ValidateAudience = true,
                         ValidateLifetime = false,
                         ValidateIssuerSigningKey = true,
                         ValidIssuer = Configuration["Jwt:Issuer"],
                         ValidAudience = Configuration["Jwt:Issuer"],
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                     };
                 });


            services.AddHttpContextAccessor();

            services.AddAutoMapperWithProfile();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IScheduledTaskRepository, ScheduledTaskRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.Configure<EmailSenderOptions>(Configuration.GetSection("Mail"));
            services.AddScoped<IEmailSender, SMTPEmailSender>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseGlobalExceptionHandler();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.RollingFile(Path.Combine(env.ContentRootPath, "Logs\\Logs-{Date}.txt"))
                .CreateLogger();

            loggerFactory.AddSerilog();

            app.UseCors("CorsPolicy");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
