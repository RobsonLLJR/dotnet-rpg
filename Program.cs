global using dotnet_rpg.Models;
global using dotnet_rpg.Services.CharacterService;
global using dotnet_rpg.Dtos.Character;
global using Microsoft.EntityFrameworkCore;
global using dotnet_rpg.Data;
var builder = WebApplication.CreateBuilder(args);

var connectionsString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(options =>
 options.UseSqlServer("Server=DESKTOP-5N9SF6F\\SQLEXPRESS;Database=dotnet-rpg;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;Encrypt=True"));
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddScoped<ICharacterService, CharacterService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();