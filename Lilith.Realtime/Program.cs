using Lilith.Realtime.Filters;
using Lilith.Realtime.Hubs;
using Lilith.Realtime.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Agregar SignalR
builder.Services.AddSignalR();

// Registro de IConnectionMultiplexer
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse("localhost:6379", true); // Cambia localhost:6379 según tu configuración
    return ConnectionMultiplexer.Connect(configuration);
});

// Agrega RedisService al contenedor
builder.Services.AddScoped<RedisService>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidateModelFilter>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Redis Cache API",
        Version = "v1",
        Description = "API para interactuar con Redis y notificaciones en tiempo real con SignalR.",
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<RedisHub>("/redisHub");

app.Run();
