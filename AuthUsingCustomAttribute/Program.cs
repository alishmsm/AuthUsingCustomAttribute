using System;
using System.Linq;
using AuthUsingCustomAttribute.Application.Attributes;
using AuthUsingCustomAttribute.Application.DTOs;
using AuthUsingCustomAttribute.Domain.Enums;
using AuthUsingCustomAttribute.Infrastructure.Middlewares;
using AuthUsingCustomAttribute.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "JWT API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token in the format: Bearer {your token}"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddEndpointsApiExplorer();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); 
    app.UseSwaggerUI(); 
}

app.UseMiddleware<AuthMiddleware>();


app.MapPost("/token", (TokenRequestDto dto) =>
{
    var token = TokenService.GenerateToken(dto.UserId, dto.RoleId);
    return Results.Ok(new { token });
});

app.MapGet("/admin", () => "Only for Admin or SuperAdmin")
    .WithMetadata(new AuthorizeRolesAttribute(
        UserRoleEnum.Admin, 
        UserRoleEnum.SuperAdmin));

app.MapGet("/public", () => "This is public");


app.Run();
