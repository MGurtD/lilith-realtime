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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Redis Cache API",
        Version = "v1",
        Description = "API para interactuar con Redis utilizando tags y notificaciones en tiempo real con SignalR.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Lilith.Realtime",
            Email = "dev@zenith.io",
            Url = new Uri("https://zenith.io")
        }
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<RedisHub>("/redisHub");

app.Run();
