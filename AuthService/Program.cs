using System.Text;
using AuthService.Data;
using AuthService.Models;
using AuthService.Profiles;
using AuthService.Services.Jwt;
using AuthService.Services.Options;
using AuthService.Services.Senders;
using AuthService.Services.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using EmailSender = AuthService.Services.Senders.EmailSender;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

Configure(app);

app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddAutoMapper(typeof(RegisterProfile));
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        });
        options.OperationFilter<SecurityRequirementsOperationFilter>();
    });
    
    services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
    services.AddTransient<ISmtpClient, SmtpClient>();
    services.AddTransient<IEmailSender, EmailSender>();
    
    services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
    services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);
    services.AddTransient<IJwtService, JwtService>();

    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection")));
    services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    services.AddAuthorization();

    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["JwtSettings:Issuer"],
            ValidAudience = configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]))
        };
    });

    services.AddControllers();
}

void Configure(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
}
