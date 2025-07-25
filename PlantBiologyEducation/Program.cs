﻿using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Plant_BiologyEducation.Data;
using Plant_BiologyEducation.Entity.Model;
using Plant_BiologyEducation.Repository;
using Plant_BiologyEducation.Service;
using PlantBiologyEducation.Service;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// CORS - Allow frontend to call API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});





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

builder.Services.AddSingleton<FcmTokenStore>();

//try
//{
//    string firebaseServiceAccountJson = null;
//    firebaseServiceAccountJson = builder.Configuration["FIREBASE_SERVICE_ACCOUNT_JSON"];

//    // Nếu không tìm thấy trong biến môi trường, thử đọc từ User Secrets (dùng cho Development)
//    if (string.IsNullOrEmpty(firebaseServiceAccountJson))
//    {
//        firebaseServiceAccountJson = builder.Configuration["Firebase:ServiceAccountJson"];
//    }
//    if (string.IsNullOrEmpty(firebaseServiceAccountJson))
//    {
//        var firebaseServiceAccountPath = Path.Combine(builder.Environment.ContentRootPath, "firebase-adminsdk.json");
//        if (File.Exists(firebaseServiceAccountPath))
//        {
//            firebaseServiceAccountJson = File.ReadAllText(firebaseServiceAccountPath);
//            Console.WriteLine($"Firebase Service Account loaded from local file: {firebaseServiceAccountPath}");
//        }
//    }

//    // Kiểm tra xem đã có JSON để khởi tạo chưa
//    if (string.IsNullOrEmpty(firebaseServiceAccountJson))
//    {
//        Console.WriteLine("Error: Firebase Service Account JSON is not configured. " +
//                          "Please set 'FIREBASE_SERVICE_ACCOUNT_JSON' as an environment variable (for production) " +
//                          "or 'Firebase:ServiceAccountJson' in user secrets (for development), " +
//                          "or ensure 'firebase-adminsdk.json' exists locally and is accessible.");
//        Environment.Exit(1); // Dừng ứng dụng nếu không thể cấu hình Firebase
//    }
//    FirebaseApp.Create(new AppOptions()
//    {
//        Credential = GoogleCredential.FromJson(firebaseServiceAccountJson)
//    });
//    Console.WriteLine("Firebase Admin SDK initialized successfully.");
//}
//catch (Exception ex)
//{
//    Console.WriteLine($"Critical Error initializing Firebase Admin SDK: {ex.Message}");
//    Console.WriteLine(ex.ToString()); // In ra chi tiết lỗi để gỡ lỗi
//    Environment.Exit(1); // Dừng ứng dụng nếu có lỗi nghiêm trọng khi khởi tạo Firebase
//}

// Repository
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<BookRepository>();
builder.Services.AddScoped<ChapterRepository>();
builder.Services.AddScoped<LessonRepository>();

builder.Services.AddScoped<Plant_Biology_Animal_Repository>();

builder.Services.AddScoped<PlantNetService>();

builder.Services.AddScoped<AccessBookRepository>();
builder.Services.AddScoped<AccessLessonRepository>();
builder.Services.AddScoped<AccessBiologyRepository>();


builder.Services.AddScoped<PredictService>();
builder.Services.AddScoped<DictionaryService>();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<EmailService>();


builder.Services
    .AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    });

// Swagger - Enable for all environments
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Plant Biology Education API",
        Version = "v1",
        Description = "API for Plant Biology Education Platform"
    });

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

// HTTP Client
builder.Services.AddHttpClient("PlantNetClient", client =>
{
    client.BaseAddress = new Uri("https://my-api.plantnet.org/v2/");
    client.Timeout = TimeSpan.FromMinutes(5);
});

// HttpClient with extended timeout for Predict API
builder.Services.AddHttpClient("PredictAPI", client =>
{
    client.Timeout = TimeSpan.FromMinutes(5); // ⏱ Tăng timeout lên 5 phút
});
builder.Services.AddScoped<PredictService>();


// Database
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Configure to listen on PORT provided by Render (fallback to 8080 locally)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

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

// Enable Swagger for all environments (useful for production API testing)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Plant Biology Education API v1");
    c.RoutePrefix = "swagger"; // Access at /swagger
});

// CORS
app.UseCors("AllowAll");

// HTTPS Redirection (comment out if having issues in production)
if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

// Map Controllers
app.MapControllers();

// Add a simple home page
app.MapGet("/", () => Results.Json(new
{
    message = "🌱 Plant Biology Education API is running!",
    documentation = "/swagger",
    version = "v1.0",
    status = "online",
    timestamp = DateTime.UtcNow
})).ExcludeFromDescription();

// Health check endpoint
app.MapGet("/health", () => Results.Json(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName
})).ExcludeFromDescription();


// API info endpoint
app.MapGet("/api", () => Results.Json(new
{
    name = "Plant Biology Education API",
    version = "1.0",
    documentation = "/swagger",
    endpoints = new
    {
        swagger = "/swagger",
        health = "/health",
        api_root = "/api"
    }
})).ExcludeFromDescription();

app.Run();