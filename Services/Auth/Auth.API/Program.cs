﻿using Auth.API;
using Auth.Data.Data;
using Auth.Data.Data.Extensions;
using Auth.Repositories;
using Auth.Application;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // Đăng ký các controller
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AuthDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthDBContext")));



builder.Services
    .AddApplicationServices(builder.Configuration)
    .AddRepositoryServices(builder.Configuration)
    .AddPresentationServices(builder.Configuration);


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthDBContext>();
    dbContext.Database.Migrate(); // Áp dụng migration tự động khi app chạy
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UsePresentationServices();

// Migration và tự động seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.InitializeAsync(services);
    var context = scope.ServiceProvider.GetRequiredService<AuthDBContext>();
    // await context.Database.MigrateAsync();
}

//app.UseMiddleware<JwtBehavior>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllers(); // Bắt buộc phải gọi để ánh xạ các controller

app.Run();
