
using DogHouse.Data;
using DogHouse.Middleware;
using DogHouse.Repositories;
using DogHouse.Services;
using Microsoft.EntityFrameworkCore;

namespace DogHouse
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            builder.Services.AddControllers();

            builder.Services.AddOpenApi();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.AddScoped<IDogRepository, DogRepository>();
            builder.Services.AddScoped<IDogService, DogService>();

            builder.Services.Configure<RateLimitingOptions>(builder.Configuration.GetSection("RateLimiting"));

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
                if (!db.Dogs.Any())
                {
                    db.Dogs.AddRange(new DogHouse.Models.Dog { Name = "Neo", Color = "red&amber", TailLength = 22, Weight = 32 },
                        new DogHouse.Models.Dog { Name = "Jessy", Color = "black&white", TailLength = 7, Weight = 14 });
                    db.SaveChanges();
                }
            }
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseMiddleware<RateLimitingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
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
