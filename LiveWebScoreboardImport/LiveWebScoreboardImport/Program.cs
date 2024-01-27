
var builder = WebApplication.CreateBuilder( args );
var currentEnv = Environment.GetEnvironmentVariable( "ASPNETCORE_ENVIRONMENT" );

// Configure logging
var logSectionConfig = builder.Configuration.GetSection( "Logging" );
builder.Logging.AddFile( logSectionConfig );

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.Logger.LogInformation( "LiveWebScoreboardImport starting with build version 2024-01-22" );

// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment() ) {
	app.UseSwagger();
	app.UseSwaggerUI();
	app.Logger.LogInformation( "LiveWebScoreboardImport UseSwaggerUI active" );
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

