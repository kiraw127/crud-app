using CarRental.Api.Data;
using CarRental.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.InitializeDatabaseAsync();

app.Run();
