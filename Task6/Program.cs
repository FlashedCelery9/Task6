using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Task6.data;
using Task6.Filters;
using Task6.midleware;
using Task6.Models;
using Task6.Services;
using Task6.Services.Participant;
using Task6.Services.TempServices;
using Task6.Services.Token;
using Task6.Services.UserService;
using Task6.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<MeetingsDBContext>(options => options.
UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(cfg => { }, typeof(Program));
builder.Services.AddHttpContextAccessor();// НЕ ЗАБУТИ
//Services
builder.Services.AddScoped<IParticipantService, ParticipantService>();
builder.Services.AddScoped<RoleService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<OldUsersService>();
builder.Services.AddSingleton<IFileUrlBuilder, FileUrlBuilder>(); //ТУТ!!!!!
builder.Services.AddScoped<IMeetingService, MeetingService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<MeetingPermisionService>();

builder.Services.AddScoped<ITokenService, JwtTokenService>();
// Фільтр валідації для всіх DTO
builder.Services.AddScoped(typeof(ValidationFilter<>));
builder.Services.AddValidatorsFromAssemblyContaining<MeetingCreateDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
// builder.Services.AddScoped<FileValidationException>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
options.IncludeXmlComments(xmlPath);

options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
{
Name = "Authorization",
Type = SecuritySchemeType.Http,
Scheme = "bearer",
BearerFormat = "JWT",
In = ParameterLocation.Header,
Description = "Вставте лише сам токен без слова Bearer."
});

options.AddSecurityRequirement(c => new OpenApiSecurityRequirement
{
{
new OpenApiSecuritySchemeReference("Bearer", c),
new List<string>()
}
});
});

// Налаштування Identity (без Cookies - тільки UserManager / SignInManager / Roles)
// AddIdentityCore не реєструє cookie-схеми автентифікації, тому єдиною схемою лишається JWT.
builder.Services.AddIdentityCore<AppUser>(options =>
{
options.SignIn.RequireConfirmedEmail = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<MeetingsDBContext>()
.AddSignInManager()
.AddDefaultTokenProviders();

var jwt = builder.Configuration.GetSection("Jwt");

// Тільки JWT - без Cookie
builder.Services
.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
// Не перейменовувати claims із коротких імен у довгі WS-Federation URI
options.MapInboundClaims = false;

options.Events = new JwtBearerEvents
{
OnChallenge = async context =>
{
context.HandleResponse();
context.Response.StatusCode = StatusCodes.Status401Unauthorized;
context.Response.ContentType = "application/json";
await context.Response.WriteAsJsonAsync(new { error = "Unauthorized", message = "Invalid or missing token" });
},
OnForbidden = async context =>
{
context.Response.StatusCode = StatusCodes.Status403Forbidden;
context.Response.ContentType = "application/json";
await context.Response.WriteAsJsonAsync(new { error = "Forbidden", message = "Insufficient permissions" });
}
};
options.TokenValidationParameters = new TokenValidationParameters
{
ValidateIssuer = true,
ValidIssuer = jwt["Issuer"],

ValidateAudience = true,
ValidAudience = jwt["Audience"],
ValidateLifetime = true,
ClockSkew = TimeSpan.Zero,

ValidateIssuerSigningKey = true,
IssuerSigningKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(jwt["Key"]!)),

NameClaimType = JwtRegisteredClaimNames.Sub,
RoleClaimType = "role"
};
});

builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{ 
// app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MeetingsDBContext>();
    context.Database.Migrate();
    SeedsData.Initialize(context);
}
app.UseAuthentication(); // ← хто ти? заповнює HttpContext.User
app.UseAuthorization();


app.UseStaticFiles(new StaticFileOptions 
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "uploads")
    ),
    RequestPath = "/uploads"
});
// app.UseMiddleware<Midleware>(); //MidleWare Homework


app.MapControllers();
app.Run();
