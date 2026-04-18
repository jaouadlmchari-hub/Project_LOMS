
namespace LOMS_Employee_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("EmployeeDbConnection");
            LOMS_Employee_DataAccess.clsDataAccessSettings.ConnectionString = connectionString;

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Après builder.Build()
            var salaryUrl = builder.Configuration["Microservices:SalaryServiceUrl"];
            var leaveUrl = builder.Configuration["Microservices:LeaveServiceUrl"];

            // Initialisation des clients statiques
            clsSalaryServiceClient.Initialize(salaryUrl);
            clsLeaveServiceClient.Initialize(leaveUrl);

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "LOMS Employee API V1");
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
