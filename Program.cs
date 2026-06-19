using Microsoft.EntityFrameworkCore;
using proyectSystemTh.Data;
using proyectSystemTh.Services.AccessUsers;
using proyectSystemTh.Services.InfoUsers;
using proyectSystemTh.Services.Solicitudes;
using proyectSystemTh.Services.UsuariosSistema;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("conexion"),
        new MySqlServerVersion(new Version(8, 0, 33)))
    );

//Secciones de Scopped
builder.Services.AddScoped<IUsuariosSistema, UsuariosSistema>();
builder.Services.AddScoped<IAccessUsers, AccessUsers>();
builder.Services.AddScoped<IInfoUsers, InforUsers>();
builder.Services.AddScoped<ISolicitudesService, SolicitudesService>();

// Add services to the container.
builder.Services.AddControllers();

//Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAll");
}
else
{
    app.UseCors("AllowAngularApp"); 
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
