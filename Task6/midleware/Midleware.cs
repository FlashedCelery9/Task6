using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Task6.midleware;

public class Midleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public Midleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var paramentr = _configuration.GetValue<bool>("Maintenance:Enabled");
        if (paramentr)
        {
            var problem = new ProblemDetails
            {
                Status = 503,
                Title = "Service Unavailable",
                Detail = "The service is temporarily unavailable due to maintenance.",
                Instance = context.Request.Path
            };
            context.Response.StatusCode = 503;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));    
            return;
        }
        await _next(context);
    }
    
}