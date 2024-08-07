using FluentValidation.AspNetCore;
using FluentValidation;
using backend.Validators;
using backend.Middlewares;
using backend.Data;
using backend.Repositories;
using backend.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
  options.AddPolicy("AllowSpecificOrigin",
    builder => {
      builder
        .WithOrigins("http://localhost:3000")
        .AllowAnyMethod()
        .AllowAnyHeader();
      }
    );
});

builder.Services.AddSingleton<DbContext>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IPositionRepository, PositionRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

builder.Services.AddControllers();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<DepartmentValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PositionValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<EmployeeValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigin");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<ErrorMiddleware>();
app.Run();
