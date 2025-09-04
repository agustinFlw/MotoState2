var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

// ?? Redirige la raíz "/" hacia la página MotosEnTaller.cshtml
app.MapGet("/", context =>
{
    context.Response.Redirect("/MotosEnTaller");
    return Task.CompletedTask;
});

app.Run();
