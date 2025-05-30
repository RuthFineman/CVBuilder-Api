using Amazon.S3;
using CVBuilder.Core.Repositories;
using CVBuilder.Core.Services;
using CVBuilder.Data.Repositories;
using CVBuilder.Data;
using CVBuilder.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using CVBuilder.Core.Validators;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
var configuration = builder.Configuration;
var apiKey = configuration["OpenAI_ApiKey"];

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}



//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//builder.Services.AddDbContext<CVBuilderDbContext>(options =>
//    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 41))));


var connectionString = builder.Configuration["ConnectionString:connectDB"];
builder.Services.AddDbContext<CVBuilderDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), options => options.CommandTimeout(60)));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddTransient<UserValidator>();
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddScoped<IFileCVService, FileCVService>();
builder.Services.AddScoped<IFileCVRepository,FileCVRepository>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
IConfiguration Configuration = builder.Configuration;
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IStatisticsRepository, StatisticsRepository>();
//builder.Services.AddDbContext<YourDbContext>(options =>
//    options.UseMySql(
//        builder.Configuration.GetConnectionString("DefaultConnection"),
//        new MySqlServerVersion(new Version(8, 0, 32))
//    ));


builder.Services.AddAuthentication(options =>
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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", builder => builder.WithOrigins("http://localhost:3000", "http://localhost:4000", "http://localhost:4200")
                                                          .AllowAnyMethod()
                                                          .AllowAnyHeader());
});
builder.Services.AddSwaggerGen(options =>
{
    options.SupportNonNullableReferenceTypes(); 
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });

});
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseCors("AllowLocalhost");
app.UseAuthentication();

app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CVBuilder API V1");
    c.RoutePrefix = string.Empty;
});
app.Run();
