using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using TestMVC.Data;
using TestMVC.Services;
using BGProcess.Services;
using BGProcess.Interface;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddDbContext<TestMVCContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TestMVCContext")));

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<UsersService>();

builder.Services.AddSingleton<IEmailQueue, EmailQueueService>();
builder.Services.AddSingleton<EmailQueueService>();

builder.Services.AddSingleton<EmailService>();

// Register EmailService as a background service
builder.Services.AddHostedService<EmailService>();

//builder.Services.AddHostedService<BGProcess.Test.TestBackgroundService>();

var app = builder.Build();

// Configure the middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Users}/{action=Index}/{id?}");

app.Run();
