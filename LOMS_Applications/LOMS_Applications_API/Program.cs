
using LOMS_Applications_DataAccess;

namespace LOMS_Applications_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("ApplicationDbConnection");
            LOMS_Applications_DataAccess.clsDataAccessSettings.ConnectionString = connectionString;
            // Ajoute "ExternalServices:" devant le nom de la clé
            string employeeUrl = builder.Configuration["ExternalServices:EMPLOYEE_API_URL"] ?? "http://localhost:7175/";
            string authUrl = builder.Configuration["ExternalServices:AUTH_API_URL"] ?? "http://localhost:7176/";

            // Injection
            EmployeeServiceClient.BaseUrl = employeeUrl;
            AuthServiceClient.baseUrl = authUrl;
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Supprime ou commente le "if" pour que Swagger soit TOUJOURS disponible
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "LOMS Application API V1");
                c.RoutePrefix = string.Empty; // Swagger s'ouvrira directement sur http://localhost:7175
            });


            // IMPORTANT : Commente cette ligne pour Docker
            // app.UseHttpsRedirection(); 

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
