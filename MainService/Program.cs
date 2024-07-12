using MainService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure services
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Apply migrations and seed database
Configure(app);

// Run the application
app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Configure the DbContext with the connection string
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

    // Add controllers to the service collection
    services.AddControllers();

    // Optionally add Swagger for API documentation
    services.AddSwaggerGen();
}

void Configure(WebApplication app)
{
    // Apply migrations at runtime
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }

    // Configure middleware
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthentication(); // Uncomment if using authentication
    app.UseAuthorization();
    
    app.MapControllers(); // Ensure this line is included to map controller routes
}