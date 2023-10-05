using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Configure Swagger for the application.
builder.Services.AddSwaggerGen(options =>
{
    // Define the security scheme for the API.
    // Here we are using OAuth2 with JWT Bearer token authentication.
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header, // Token will be sent in the header.
        Name = "Authorization", // The name of the header parameter.
        Type = SecuritySchemeType.Http, // Authentication type.
        Scheme = "bearer", // The scheme, in this case, bearer token. Note: it's important that "bearer" is lowercase.
    });

    // Apply security requirements to operations globally.
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

// Configure JWT authentication for the application.
builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true, // Ensure that the key used to sign the incoming token is a valid key.
        ValidateAudience = false, // Don't validate the token's intended audience.
        ValidateIssuer = false, // Don't validate the token's issuer.

        // Use the symmetric key for token validation.
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            // Fetch the secret key from the configuration.
            builder.Configuration.GetSection("AppSettings:Token").Value!))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Fetch the token from the request headers.
            var token = context.Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrEmpty(token))
            {
                // If the token string starts with "Bearer ", strip it off.
                if (token.StartsWith("Bearer "))
                {
                    token = token.Substring("Bearer ".Length);
                }
                context.Token = token; // Set the token for the current context.
            }

            // Return a completed task since this is non-blocking.
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

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