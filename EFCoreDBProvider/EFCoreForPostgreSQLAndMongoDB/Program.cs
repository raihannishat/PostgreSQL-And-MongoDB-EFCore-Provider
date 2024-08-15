var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register settings for mongodb context
var mongoDbSettings = builder.Configuration.GetSection(typeof(MongoDbSettings).Name).Get<MongoDbSettings>();
builder.Services.AddDbContext<MongoDbContext>(options =>
    options.UseMongoDB(mongoDbSettings!.AtlasURI, mongoDbSettings.DatabaseName));

// Register settings for pgsqldb context
var pgsqlSettings = builder.Configuration.GetSection(typeof(PgsqlDbSettings).Name).Get<PgsqlDbSettings>();
builder.Services.AddDbContext<PostgreSqlDbContext>(options =>
    options.UseNpgsql(pgsqlSettings!.ConnectionString));

// Register service collection extension
builder.Services.AddConfigurations();

var app = builder.Build();

// Apply migration for PostgreSQL
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PostgreSqlDbContext>();

    try
    {
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Handle migration exceptions
        Console.WriteLine($"An error occurred while applying migrations: {ex.Message}");
    }
}

// Add custom endpoint
UserEndpoints.MapEndpoints(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
