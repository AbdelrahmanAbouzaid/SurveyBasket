
using Serilog;

namespace SurveyBasket.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddServices(builder.Configuration);

            builder.Host.UseSerilog((context, config) =>
                config.ReadFrom.Configuration(context.Configuration)
            );
          
            var app = builder.Build();

            app.UseMiddlewares();

            app.Run();
        }
    }
}
