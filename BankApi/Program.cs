
using BankApi.Data;
using BankApi.Services;
using Microsoft.EntityFrameworkCore;

namespace BankApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<IAccountServices, AccountServices>();
            builder.Services.AddScoped<ICustomerServices, CustomerServices>();

            var ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Add services to the container.
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(ConnectionString));
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(); 
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
