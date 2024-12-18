using Lilith.Realtime.Filters;
using Lilith.Realtime.Hubs;
using Lilith.Realtime.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Agregar SignalR
builder.Services.AddSignalR();

// Cargar configuración de appsettings.json y variables de entorno
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                     .AddEnvironmentVariables();

// Agregar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        policy.WithOrigins(allowedOrigins ?? ["http://localhost:8100"]) // Usar origen de appsettings o localhost como fallback
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Habilitar credenciales
    });
});

// Registro de IConnectionMultiplexer
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisConnectionString = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379";
    var configuration = ConfigurationOptions.Parse(redisConnectionString, true); // Cambia localhost:6379 según tu configuración
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

// global cors policy
app.UseCors("CorsPolicy"); // Aplicar la política CORS

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.MapHub<RedisHub>("/redis");

app.Run();
