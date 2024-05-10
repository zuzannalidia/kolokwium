using Kolokwium.Data;
using Kolokwium.Options;
using Microsoft.Extensions.Options;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        builder.Services.Configure<DataOptions>(builder.Configuration.GetSection("Data"));
        builder.Services.AddScoped<IDataContext, DataContext>();
        
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
        
        await using (var scope = app.Services.CreateAsyncScope())
        {
            var options = scope.ServiceProvider.GetRequiredService<IOptions<DataOptions>>();
            await DataContext.InitializeDb(options);
        }

        await app.RunAsync();
    }
}