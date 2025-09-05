using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Read the JWT secret key from the appsettings.json configuration file.
// This key will be used to sign and validate JWT tokens.
var jwtSecretKey = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSecretKey.GetValue<string>("SecretKey") ?? "d06e9577107f86a83a2698ce2fa7a998c94e3ed45eefbb65";

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Keep property names as defined in the model
    });

// Register authentication services with JWT Bearer scheme
builder.Services.AddAuthentication(options =>
{
    // Set the default authentication scheme to JWT Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

    // Set the default challenge scheme to JWT Bearer
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        // Configure JWT Bearer options
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            // Validate the signing key
            ValidateIssuerSigningKey = true,

            // Set the signing key using the secret key
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),

            // Do not validate the issuer
            ValidateIssuer = false,

            // Do not validate the audience
            ValidateAudience = false,

            // Do not validate token lifetime
            ValidateLifetime = false
        };
    });

builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
