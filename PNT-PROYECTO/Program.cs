﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PNT_PROYECTO.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<PNT_PROYECTOContext>(options =>
    options.UseSqlite(@"filename=C:\Temp\Stock1.db"));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opciones =>
    {
        opciones.LoginPath = "/Home/Index";
        opciones.AccessDeniedPath = "/Usuarios/NoAutorizado";
        opciones.LogoutPath = "/Login/Logout";
        opciones.ExpireTimeSpan = System.TimeSpan.FromMinutes(10);
    });

builder.Services.AddSession();

var app = builder.Build();

app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
