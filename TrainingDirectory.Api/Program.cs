WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
WebApplication? app = builder.Build();
app.UseHttpsRedirection();

app.MapGet("/", () => Results.Ok(new { Status = "Healthy", Service = "Training Directory API" }));

app.MapControllers();
app.Run();
