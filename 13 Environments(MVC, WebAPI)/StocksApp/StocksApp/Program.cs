var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
builder.Services.AddControllersWithViews();

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
