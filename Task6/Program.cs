using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Task6.data;
using Task6.Filters;
using Task6.midleware;
using Task6.Models;
using Task6.Services;
using Task6.Services.TempServices;
using Task6.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<MeetingsDBContext>(options => options.
    UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(cfg => { }, typeof(Program));
builder.Services.AddHttpContextAccessor();// НЕ ЗАБУТИ
//Services
// builder.Services.AddScoped<IParticipantService, ParticipantService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<OldUsersService>();
builder.Services.AddSingleton<IFileUrlBuilder, FileUrlBuilder>(); //ТУТ!!!!!
builder.Services.AddScoped<IMeetingService, MeetingService>();
builder.Services.AddScoped<IAuthService, AuthService>();
// Фільтр валідації для всіх DTO
builder.Services.AddScoped(typeof(ValidatorFilter.ValidationFilter<>));
builder.Services.AddValidatorsFromAssemblyContaining<MeetingCreateDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<MeetingsDBContext>() //Вказівник на БД. Без нього нічого не працюватиме
    .AddDefaultTokenProviders();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{   
    app.MapOpenApi();
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
