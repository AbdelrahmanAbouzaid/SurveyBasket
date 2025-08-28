using FluentValidation.AspNetCore;
using Hangfire;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SurveyBasket.Api.Authentication;
using SurveyBasket.Api.Middlewares;
using SurveyBasket.Api.Persistence;
using SurveyBasket.Api.Settings;
using System.Reflection;
using System.Text;

namespace SurveyBasket.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            services.AddOpenApi();

            services.AddScoped<IPollServices, PollServices>();
            services.AddScoped<IAuthServices, AuthServices>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IVoteService, VoteService>();
            services.AddScoped<IResultService, ResultService>();
            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUserService, UserService>();

            services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));

            services.AddFluentValidationServices();

            services.AddIdentityServices(configuration);

            services.AddMapsterServices();

            services.AddDbContextServices(configuration);

            services.AddCorsServices();

            services.AddIdentityOptionServices();

            services.AddExceptionHandler<GlobalHandlingExceptionMiddleware>();
            services.AddProblemDetails();

            services.AddDistributedMemoryCache();

            services.AddHttpContextAccessor();

            services.AddBAckgroundJobsServices(configuration);

            return services;
        }

        private static IServiceCollection AddIdentityOptionServices(this IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            });
            return services;
        }
        private static IServiceCollection AddCorsServices(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });
            return services;
        }
        private static IServiceCollection AddMapsterServices(this IServiceCollection services)
        {
            var mappinConfig = TypeAdapterConfig.GlobalSettings;
            mappinConfig.Scan(Assembly.GetExecutingAssembly());
            services.AddSingleton<IMapper>(new Mapper(mappinConfig));

            return services;
        }
        private static IServiceCollection AddFluentValidationServices(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation()
               .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            return services;
        }
        private static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IJwtProvider, JwtProvider>();
            //services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
            services.AddOptions<JwtOptions>()
                .BindConfiguration(JwtOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var settings = configuration.GetRequiredSection(JwtOptions.SectionName).Get<JwtOptions>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,

                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings!.Key)),
                    ValidIssuer = settings.Issuer,
                    ValidAudience = settings.Audience
                };
            });
            return services;
        }
        private static IServiceCollection AddDbContextServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            });
            return services;
        }

        private static IServiceCollection AddBAckgroundJobsServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(config => config
               .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
               .UseSimpleAssemblyNameTypeSerializer()
               .UseRecommendedSerializerSettings()
               .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection"))); services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));

            return services;    
        }


        public static WebApplication UseMiddlewares(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
                app.UseHangfireDashboard("/jobs");
            }

            app.UseSerilogRequestLogging();

            app.UseExceptionHandler();

            app.UseDataSeedingMiddleware();

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseAuthorization();


            app.MapControllers();

            return app;
        }

        private static WebApplication UseDataSeedingMiddleware(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
            dbInitializer.InitializeAsync().Wait();
            return app;
        }
    }
}
