var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapGet("/{ruleset}/{state}/{moves}/api", (string ruleset, string state, string moves) =>
{
    // Your logic to return JSON data
    return Results.Json(new { ruleset, state, moves });
});

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
