using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ProductOrder.Application.Validation;
using ProductOrderApi;
using ProductOrderApi.Abstractions;
using ProductOrderApi.Abstractions.ProductOrder.Application.Abstractions;
using ProductOrderApi.Repositories;
using ProductOrderApi.Services;

var builder = WebApplication.CreateBuilder(args);

// DbContext (SQL Server example)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add controllers
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Repositories + Unit of Work
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Current user service for auditing
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Application services
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<OrderService>();

// Validation (FluentValidation)
builder.Services.AddValidatorsFromAssemblyContaining<ProductCreateValidator>();

// ProblemDetails middleware for standardized error responses
builder.Services.AddProblemDetails();

// CORS (optional, configure as needed)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseCors();
app.MapControllers();

app.Run();