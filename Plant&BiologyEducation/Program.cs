using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Plant_BiologyEducation.Data;
using Plant_BiologyEducation.Repository;
using Plant_BiologyEducation.Service;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// JWT Service
builder.Services.AddScoped<JwtService>();

// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
        RoleClaimType = System.Security.Claims.ClaimTypes.Role
    };
});



// Repository
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<BookRepository>();
builder.Services.AddScoped<ChapterRepository>();
builder.Services.AddScoped<LessonRepository>();

builder.Services.AddScoped<ManageBookRepository>();
builder.Services.AddScoped<ManageChapterRepository>();
builder.Services.AddScoped<ManageLessonRepository>();

builder.Services.AddScoped<AccessBookHistoryRepository>();
builder.Services.AddScoped<AccessLessonHistoryRepository>();

builder.Services.AddScoped<Plant_Biology_Animal_Repository>();

builder.Services.AddScoped<PlantNetService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Plant Biology Education API", Version = "v1" });

    // JWT Authentication configuration for Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// 
builder.Services.AddHttpClient("PlantNetClient", client =>
{
    client.BaseAddress = new Uri("https://my-api.plantnet.org/v2/");
});


// Database
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Test database connection
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        try
        {
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            var canConnect = context.Database.CanConnect();
            Console.WriteLine($"Database connection: {(canConnect ? "SUCCESS" : "FAILED")}");

            if (!canConnect)
            {
                Console.WriteLine("Connection string: " + builder.Configuration.GetConnectionString("DefaultConnection"));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database connection error: {ex.Message}");
            Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();