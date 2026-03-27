using Microsoft.AspNetCore.Mvc;
using QuickToCash.Api.Common;
using QuickToCash.Api.Middleware;
using QuickToCash.Api.Repositories;
using QuickToCash.Api.Repositories.Interfaces;
using QuickToCash.Api.Services;
using QuickToCash.Api.Services.Interfaces;

const string LocalFrontendCorsPolicy = "LocalFrontendCorsPolicy";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy(LocalFrontendCorsPolicy, policy =>
    {
        policy
            .WithOrigins("http://localhost:4200", "https://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Invalid request." : e.ErrorMessage)
            .ToArray();

        return new BadRequestObjectResult(ApiResponse<object>.Fail("Validation failed.", errors));
    };
});

builder.Services.AddSingleton<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddSingleton<IEarlyPaymentRequestRepository, EarlyPaymentRequestRepository>();
builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddScoped<IEarlyPaymentService, EarlyPaymentService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors(LocalFrontendCorsPolicy);
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
