namespace LOMS_Auth_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("AuthDbConnection");
            LOMS_Auth_DataAccess.clsDataAccessSettings.ConnectionString = connectionString;

            var employeeUrl = builder.Configuration["ExternalServices:EmployeeApiUrl"];

            if (!string.IsNullOrEmpty(employeeUrl))
            {
                // On envoie l'URL à la classe DataAccess
                LOMS_Auth_DataAccess.LOMS_Auth_DataAccess.clsEmployeeServiceClient.BaseUrl = employeeUrl;
            }

            // --- AJOUT CORS ICI ---
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:5000", // Port Vite actuel
                            "http://localhost:5173", // Port Vite par défaut
                            "http://localhost:8080"  // Au cas où tu testes via Nginx direct
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            // ----------------------

            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "LOMS Auth API V1");
                c.RoutePrefix = string.Empty;
            });

            // IMPORTANT : Commente cette ligne pour Docker si tu n'utilises pas HTTPS
            // app.UseHttpsRedirection(); 

            // --- UTILISER CORS AVANT LES CONTRÔLEURS ---
            app.UseCors("AllowFrontend");
            // -------------------------------------------

            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}