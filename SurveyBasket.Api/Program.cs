
namespace SurveyBasket.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddServices(builder.Configuration);
          
            var app = builder.Build();

            app.UseMiddlewares();

            app.Run();
        }
    }
}
