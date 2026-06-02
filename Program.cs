using Microsoft.EntityFrameworkCore;
using NEI;
using NEI.Data;
using NEI.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("OracleConnection");

builder.Services.AddDbContext<AppDbContext>
(
    options => options.UseOracle(connectionString,
    b => b.UseOracleSQLCompatibility(OracleSQLCompatibility.DatabaseVersion19))
);

builder.Services.AddHttpClient("NasaClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["NasaApi:BaseUrl"]);
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Adiciona o conversor global para transformar Enums em Strings no JSON de saída
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<NasaIntegrationService>();
builder.Services.AddScoped<CloseApproachService>();
builder.Services.AddScoped<RiskAssessmentService>();

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
